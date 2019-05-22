using Android.Content;
using Android.OS;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace iDatech.WifiP2p.Poc.WifiP2p
{
    /// <summary>
    /// A message sent over the network.
    /// </summary>
    sealed public class Message : IWifiP2pMessage
    {
        #region Constants

        /// <summary>
        /// The step to download / upload. Size of intermediate buffer = 10ko.
        /// </summary>
        private const int Step = 10 * 1024;

        #endregion Constants

        #region Static methods

        /// <summary>
        /// Receive a message from a client socket.
        /// </summary>
        /// <param name="context">The context used to send broadcasts on the progress.</param>
        /// <param name="clientSocket">The client socket.</param>
        /// <returns>The received message.</returns>
        static public Message Receive(Context context, Socket clientSocket)
        {
            // Fetch the message type (first byte)
            byte[] buffer = new byte[1];
            int bytesRead = clientSocket.Receive(buffer, 1, SocketFlags.None);

            if (bytesRead != 1)
            {
                throw new InvalidOperationException($"The received message could not be read.");
            }

            // Read message type
            EMessageType messageType = (EMessageType)buffer[0];

            // If the specified message is supposed to carry data, read it.  
            if (messageType.IsCarryingData())
            {
                // Read object size (Int64 -> 8 bytes).
                buffer = new byte[8];
                bytesRead = clientSocket.Receive(buffer, 4, SocketFlags.None);

                if (bytesRead != 8)
                {
                    throw new InvalidOperationException($"The size of the object could not be read.");
                }

                long objLength = BitConverter.ToInt64(buffer, 0);
                List<byte> objBytes = new List<byte>();
                int totalBytesRead = 0;
                string intentAction = messageType == EMessageType.SendData
                    ? WifiP2pMessageIntent.ActionReceivedDataProgress
                    : WifiP2pMessageIntent.ActionReceivedFileProgress;

                buffer = new byte[Step];
                while ((bytesRead = clientSocket.Receive(buffer, Step, SocketFlags.None)) > 0)
                {
                    totalBytesRead += bytesRead;
                    objBytes.AddRange(buffer);

                    // Broadcast progress
                    Intent progressIntent = new WifiP2pMessageIntent(intentAction, (float)totalBytesRead / objLength, false);
                    context.SendBroadcast(progressIntent);
                }

                if (totalBytesRead != objLength)
                {
                    throw new InvalidOperationException($"Error while reading the object : {bytesRead} bytes read but object should be {objLength} bytes long.");
                }

                Intent doneIntent;
                IParcelable data = null;
                switch (messageType)
                {
                    case EMessageType.SendData:
                        // Deserialize the object.
                        var formatter = new BinaryFormatter();
                        data = formatter.Deserialize(new MemoryStream(objBytes.ToArray())) as IParcelable;
                        break;
                    case EMessageType.SendFile:
                        // Write the file temporarily on the phone's internal storage.
                        string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Resources), "received_file.csv");
                        File.WriteAllBytes(filePath, objBytes.ToArray());

                        data = new ReceivedFileInfo(filePath, objBytes.ToArray());
                        break;
                    default:
                        throw new NotSupportedException($"Message {messageType} not supported");
                }                

                return new Message(messageType)
                {
                    Length = objLength,
                    Object = data
                };
            }

            return new Message(messageType);
        }

        #endregion Static methods

        #region Properties

        /// <summary>
        /// <see cref="IWifiP2pMessage.MessageType"/>
        /// </summary>
        public EMessageType MessageType { get; private set; }

        /// <summary>
        /// <see cref="IWifiP2pMessage.Length"/>
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// <see cref="IWifiP2pMessage.Object"/>
        /// </summary>
        public IParcelable Object { get; private set; }

        /// <summary>
        /// Is the current message carrying data ?
        /// </summary>
        public bool IsCarryingData => MessageType.IsCarryingData() && Object != null;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Specify the message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        public Message(EMessageType messageType)
        {
            MessageType = messageType;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Set and send the object of this message.
        /// </summary>
        /// <param name="clientSocket">The socket to send the data through.</param>
        /// <param name="obj">The object to send.</param>
        /// <param name="context">The context needed to send the broadcast.</param>
        /// <remarks>Depending on the current message type, only one object type will be supported.</remarks>
        public void Send(Socket clientSocket, IParcelable obj, Context context)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));

            var formatter = new BinaryFormatter();
            string intentAction = MessageType == EMessageType.SendData
                    ? WifiP2pMessageIntent.ActionSendDataProgress
                    : WifiP2pMessageIntent.ActionSendFileProgress;

            // First send the message type.
            byte[] buffer = new byte[1];
            buffer[0] = (byte)MessageType;
            clientSocket.SendBufferSize = buffer.Length;
            clientSocket.Send(buffer);

            // Then send the object length (in bytes)
            buffer = BitConverter.GetBytes(Length);
            clientSocket.SendBufferSize = buffer.Length;
            clientSocket.Send(buffer);
            
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                buffer = ms.ToArray();
                for (int i = 0; i < buffer.Length;)
                {
                    clientSocket.SendBufferSize = Step;
                    i += clientSocket.Send(buffer, i, Step, SocketFlags.None);
                    Intent progressIntent = new WifiP2pMessageIntent<IParcelable>(intentAction, (float)i / buffer.Length, false, obj);
                    context.SendBroadcast(progressIntent);
                }
            }

            // Broadcast completion.
            Intent doneIntent = new WifiP2pMessageIntent<IParcelable>(intentAction, 1, true, obj);
            context.SendBroadcast(doneIntent);
        }

        #endregion Methods
    }
}