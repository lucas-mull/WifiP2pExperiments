using Android.App;
using Android.Content;
using System;
using System.Net.Sockets;
using System.Threading;

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
            Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Udp);

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                // A connection has been established, increment active sockets count
                lock (m_Lock)
                {
                    m_CurrentlyActiveSockets++;
                }

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    HandleClient(clientSocket);
                });
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            if (!clientSocket.Connected)
            {
                throw new ArgumentException($"Socket is not connected.");
            }

            // Receive the full message from the client.
            Message message = Message.Receive(this, clientSocket);

            if (message.IsCarryingData)
            {

            }

            // Process the message according to the type.
            switch (message.MessageType)
            {
                // TODO process the message.
            }
        }

        #endregion Methods
    }
}