using Android.Content;
using Android.OS;
using Android.Runtime;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using Java.Interop;
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
    sealed public class Message : Java.Lang.Object, IWifiP2pMessage, IParcelable
    {
        #region Constants

        /// <summary>
        /// The step to download / upload. Size of intermediate buffer = 10ko.
        /// </summary>
        private const int Step = 10 * 1024;

        #endregion Constants

        #region Static variables

        /// <summary>
        /// The parcelable creator.
        /// </summary>
        static private readonly ParcelableCreator<Message> s_Creator = new ParcelableCreator<Message>(FromParcel);

        #endregion Static variables

        #region Static methods

        /// <summary>
        /// Receive a message from a client socket.
        /// </summary>
        /// <param name="context">The context used to send broadcasts on the progress.</param>
        /// <param name="clientSocket">The client socket.</param>
        static public void Receive(Context context, Socket clientSocket)
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

            Message msg = new Message(messageType);

            // Stop reading if the message is not carrying any data.
            if (!messageType.IsCarryingData())
            {
                return;
            }

            // Otherwise read it
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
            string intentAction = messageType.GetIntentAction(false);

            buffer = new byte[Step];
            while ((bytesRead = clientSocket.Receive(buffer, Step, SocketFlags.None)) > 0)
            {
                totalBytesRead += bytesRead;
                objBytes.AddRange(buffer);

                // Broadcast progress
                Intent progressIntent = new WifiP2pMessageIntent(intentAction, (float)totalBytesRead / objLength, false, msg);
                context.SendBroadcast(progressIntent);
            }

            if (totalBytesRead != objLength)
            {
                throw new InvalidOperationException($"Error while reading the object : {bytesRead} bytes read but object should be {objLength} bytes long.");
            }

            // Deserialize the object.
            var formatter = new BinaryFormatter();
            msg.Data = formatter.Deserialize(new MemoryStream(objBytes.ToArray())) as IParcelable;

            // Broadcast completion
            Intent doneIntent = new WifiP2pMessageIntent(intentAction, 1, true, msg);
            context.SendBroadcast(doneIntent);
        }

        /// <summary>
        /// Method used by the creator to create a message from a parcel.
        /// </summary>
        /// <param name="in">The parcel.</param>
        /// <returns>The message created from the parcel.</returns>
        static public Message FromParcel(Parcel @in)
        {
            if (@in == null)
            {
                throw new ArgumentNullException(nameof(@in));
            }

            var msg = new Message
            {
                MessageType = (EMessageType)((byte)@in.ReadByte()),
                Length = @in.ReadLong(),
                TypeLitteral = @in.ReadString()
            };

            Type objType = Type.GetType(msg.TypeLitteral);
            msg.Data = @in.ReadParcelable(Java.Lang.Class.FromType(objType).ClassLoader) as IParcelable;

            return msg;
        }

        /// <summary>
        /// The method used to access the creator for this parcelable.
        /// </summary>
        /// <returns>The parcelable creator.</returns>
        [ExportField("CREATOR")]
        static public ParcelableCreator<Message> GetCreator() => s_Creator;

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
        /// The type litteral of the object that is being sent.
        /// Needed to get the class loader to decode parcel.
        /// </summary>
        public string TypeLitteral { get; private set; }

        /// <summary>
        /// <see cref="IWifiP2pMessage.Data"/>
        /// </summary>
        public IParcelable Data { get; private set; }

        /// <summary>
        /// Is the current message carrying data ?
        /// </summary>
        public bool IsCarryingData => MessageType.IsCarryingData() && Data != null;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Parameterless constructor needed for the <see cref="IParcelable"/> interface.
        /// </summary>
        public Message() { }

        /// <summary>
        /// Default constructor.
        /// Specify the message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="data">The optional parcelable data to send alongside the message.</param>
        public Message(EMessageType messageType, IParcelable data = null)
        {
            MessageType = messageType;
            Data = data;
            TypeLitteral = data?.GetType().AssemblyQualifiedName;
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
        public void Send(Socket clientSocket, Context context)
        {
            var formatter = new BinaryFormatter();
            string intentAction = MessageType.GetIntentAction(true);

            // First send the message type.
            byte[] buffer = new byte[1];
            buffer[0] = (byte)MessageType;
            clientSocket.SendBufferSize = buffer.Length;
            clientSocket.Send(buffer);

            // Then send the object length (in bytes) if the message is carrying data.
            if (IsCarryingData)
            {
                buffer = BitConverter.GetBytes(Length);
                clientSocket.SendBufferSize = buffer.Length;
                clientSocket.Send(buffer);

                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, Data);
                    buffer = ms.ToArray();
                    for (int i = 0; i < buffer.Length;)
                    {
                        clientSocket.SendBufferSize = Step;
                        i += clientSocket.Send(buffer, i, Step, SocketFlags.None);
                        Intent progressIntent = new WifiP2pMessageIntent(intentAction, (float)i / buffer.Length, false, this);
                        context.SendBroadcast(progressIntent);
                    }
                }
            }

            // Broadcast completion.
            Intent doneIntent = new WifiP2pMessageIntent(intentAction, 1, true, this);
            context.SendBroadcast(doneIntent);
        }

        /// <summary>
        /// <see cref="IParcelable.DescribeContents"/>
        /// </summary>
        public int DescribeContents()
        {
            return GetHashCode();
        }

        /// <summary>
        /// <see cref="IParcelable.WriteToParcel(Parcel, ParcelableWriteFlags)"/>
        /// </summary>
        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteByte((sbyte)MessageType);
            dest.WriteLong(Length);
            dest.WriteString(TypeLitteral);
            dest.WriteParcelable(Data, ParcelableWriteFlags.None);
        }

        #endregion Methods
    }
}