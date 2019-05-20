using Android.App;
using Android.Content;
using System;
using System.Net.Sockets;

namespace iDatech.WifiP2p.Poc.WifiP2p.Services
{
    [Service]
    sealed public class ServerAcceptService : IntentService
    {
        #region Constants

        /// <summary>
        /// The port to listen to.
        /// </summary>
        private const int Port = 8888;

        #endregion Constants

        #region Instance variables

        /// <summary>
        /// The lock
        /// </summary>
        private object m_Lock;

        /// <summary>
        /// The amount of client sockets that are currently active (i.e. response is being handled by the server).
        /// </summary>
        private int m_CurrentlyActiveSockets;

        #endregion Instance variables

        #region Methods

        override protected void OnHandleIntent(Intent intent)
        {
            while (true)
            {
                Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Udp);

                // TODO add client handling in a different thread.
                HandleClient(serverSocket.Accept());

                // A connection has been established, increment active sockets count
                lock (m_Lock)
                {
                    m_CurrentlyActiveSockets++;
                }
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            if (!clientSocket.Connected)
            {
                throw new ArgumentException($"Socket is not connected.");
            }

            // Receive the message from the client asynchronously.
            Message message = Message.Receive(clientSocket);

            // Process the message according to the type.
            switch (message.MessageType)
            {
                // TODO process the message.
            }
        }

        #endregion Methods
    }
}