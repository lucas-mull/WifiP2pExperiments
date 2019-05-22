using Android.App;
using Android.Content;
using Android.Net.Wifi.P2p;
using Android.OS;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
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

        /// <summary>
        /// Extra parameter containing the host's IP address.
        /// </summary>
        public const string ExtraIpAddress = "EXTRA_IP_ADDRESS";

        #endregion Constants

        #region Instance variables

        /// <summary>
        /// The lock
        /// </summary>
        private object m_Lock;

        /// <summary>
        /// The server socket listening to incoming connections.
        /// </summary>
        private Socket m_ServerSocket;

        /// <summary>
        /// The amount of client sockets that are currently active (i.e. response is being handled by the server).
        /// </summary>
        private int m_CurrentlyActiveSockets;

        #endregion Instance variables

        #region Methods

        override protected void OnHandleIntent(Intent intent)
        {
            // Fetch the IP address from the intent extras.
            string targetAddress = intent.GetStringExtra(ExtraIpAddress) ?? throw new ArgumentException($"The intent did not contain the following parameter : {ExtraIpAddress}", nameof(intent));
            if (!IPAddress.TryParse(targetAddress, out IPAddress ipAddress))
            {
                throw new ArgumentException($"The specified IP address {targetAddress} could not be parsed.", nameof(intent));
            }

            m_ServerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new IPEndPoint(ipAddress, Port));
            m_ServerSocket.Listen(10);

            while (true)
            {
                Socket clientSocket = m_ServerSocket.Accept();

                // A connection has been established, increment active sockets count
                lock (m_Lock)
                {
                    m_CurrentlyActiveSockets++;
                }

                // And handle the client in a different thread.
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    HandleClient(clientSocket);
                });
            }
        }

        /// <summary>
        /// <see cref="IntentService.OnDestroy"/>
        /// </summary>
        override public void OnDestroy()
        {
            base.OnDestroy();
            m_ServerSocket.Close();
        }

        /// <summary>
        /// Handle a client's message.
        /// </summary>
        /// <param name="clientSocket">The client socket.</param>
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
                string intentAction = message.MessageType == EMessageType.SendData
                    ? WifiP2pMessageIntent.ActionReceivedDataProgress
                    : WifiP2pMessageIntent.ActionReceivedFileProgress;

                // Broadcast completion
                Intent doneIntent = new WifiP2pMessageIntent<IParcelable>(intentAction, 1, true, message.Object);
                SendBroadcast(doneIntent);
            }
            else
            {
                switch(message.MessageType)
                {
                    case EMessageType.RequestFile:
                        // TODO send file
                        break;
                    case EMessageType.RequestUpdatedData:
                        // TODO send updated data
                        break;
                }
            }

            clientSocket.Close();

            // Release the socket.
            lock(m_Lock)
            {
                m_CurrentlyActiveSockets--;
            }
        }

        #endregion Methods
    }
}