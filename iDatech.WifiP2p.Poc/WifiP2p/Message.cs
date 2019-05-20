using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using System;
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
        #region Static methods

        /// <summary>
        /// Receive a message from a client socket.
        /// </summary>
        /// <param name="clientSocket">The client socket.</param>
        /// <returns>The received message.</returns>
        static public Message Receive(Socket clientSocket)
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

            // For action PingServer, no object is sent.
            if (messageType != EMessageType.PingServer)
            {
                // Read object size (Int32 -> 4 bytes).
                buffer = new byte[4];
                bytesRead = clientSocket.Receive(buffer, 4, SocketFlags.None);

                if (bytesRead != 4)
                {
                    throw new InvalidOperationException($"The size of the object could not be read.");
                }

                int objLength = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[objLength];
                bytesRead = clientSocket.Receive(buffer, objLength, SocketFlags.None);
                if (bytesRead != objLength)
                {
                    throw new InvalidOperationException($"Error while reading the object : {bytesRead} bytes read but object should be {objLength} bytes long.");
                }

                // Deserialize the object.
                var formatter = new BinaryFormatter();
                object obj = formatter.Deserialize(new MemoryStream(buffer));

                return new Message
                {
                    MessageType = messageType,
                    Length = objLength,
                    Object = obj
                };
            }

            return new Message
            {
                MessageType = messageType
            };
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
        public int Length { get; private set; }

        /// <summary>
        /// <see cref="IWifiP2pMessage.Object"/>
        /// </summary>
        public object Object { get; private set; }

        #endregion Properties
    }
}