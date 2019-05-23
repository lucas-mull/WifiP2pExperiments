using Android.Content;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// The singleton, server-side, used to hold all the data that has been sent to / received by the clients
    /// </summary>
    sealed public class AccessPoint
    {
        #region Static variables

        /// <summary>
        /// The singleton instance.
        /// </summary>
        static public AccessPoint Instance { get; }

        #endregion Static variables

        #region Static constructor

        /// <summary>
        /// Initialize the only instance in the static constructor.
        /// </summary>
        static AccessPoint()
        {
            Instance = new AccessPoint();
        }

        #endregion Static constructor

        #region Instance variables

        /// <summary>
        /// The intent used to start the <see cref="ServerListeningService"/>.
        /// </summary>
        private Intent m_ServerServiceIntent;

        #endregion Instance variables

        #region Properties

        /// <summary>
        /// The exhaustive list of all the chat messages that were received.
        /// </summary>
        public List<ChatMessage> Messages { get; private set; }

        /// <summary>
        /// The list of pending messages, for each device (uses the mac address as key).
        /// </summary>
        public Dictionary<string, Queue<ChatMessage>> PendingMessages { get; private set; }

        /// <summary>
        /// The list of currently connected clients. Stores the mac address since it's unique to each client.
        /// </summary>
        public List<DeviceInfo> ConnectedClients { get; private set; }

        /// <summary>
        /// Whether or not the server is currently listening to incoming connections.
        /// </summary>
        public bool IsListening { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Private constructor. Initialize all the collections.
        /// </summary>
        private AccessPoint()
        {
            Messages = new List<ChatMessage>();
            PendingMessages = new Dictionary<string, Queue<ChatMessage>>();
            ConnectedClients = new List<DeviceInfo>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Register a message. Add it to the list of all messages and mark it as "to send" for all the clients that are not the sender.
        /// </summary>
        /// <param name="message">The message.</param>
        public void RegisterMessage(ChatMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            foreach(string address in ConnectedClients)
            {
                if (address != message.SenderAddress)
                {
                    lock (PendingMessages)
                    {
                        PendingMessages[address].Enqueue(message);
                    }
                }
            }
        }

        /// <summary>
        /// try to add a new client's mac address.
        /// </summary>
        /// <param name="deviceInfo">The new client's info.</param>
        /// <returns><c>true</c> if the client was added successfully, <c>false</c> if it was already registered.</returns>
        public bool TryAddClient(DeviceInfo deviceInfo)
        {
            if (ConnectedClients.Any(d => d.MacAddress == deviceInfo.MacAddress))
            {
                return false;
            }

            lock (ConnectedClients)
            {
                ConnectedClients.Add(deviceInfo);
            }

            // Initialize the pending messages for this client
            if (!PendingMessages.ContainsKey(deviceInfo.MacAddress))
            {
                lock (PendingMessages)
                {
                    PendingMessages.Add(deviceInfo.MacAddress, new Queue<ChatMessage>());
                }
            }

            return true;
        }

        /// <summary>
        /// Remove a client.
        /// </summary>
        /// <param name="clientAddress">The target client's mac address.</param>
        public void RemoveClient(string clientAddress)
        {
            lock (ConnectedClients)
            {
                ConnectedClients.Remove(ConnectedClients.First(c => c.MacAddress == clientAddress));
            }

            // Keep the pending messages for this client. If it connects again, we want to be able to send the missing messages.
        }

        /// <summary>
        /// Does the specified <paramref name="clientAddress"/> have pending messages ?
        /// </summary>
        /// <param name="clientAddress">The client's mac address.</param>
        /// <returns><c>true</c> if the client has pending messages, <c>false</c> otheriwse.</returns>
        public bool HasPendingMessages(string clientAddress)
        {
            return PendingMessages[clientAddress].Any();
        }

        /// <summary>
        /// Start listening to incoming client connections.
        /// </summary>
        /// <param name="context">The context (used to start the listener service).</param>
        /// <param name="hostAddress">The IP address of this device (host).</param>
        /// <param name="port">The port to listen on.</param>
        public void StartListening(Context context, string hostAddress, int port)
        {
            // Check if we're not already listening.
            if (!IsListening)
            {
                m_ServerServiceIntent = new Intent(context, typeof(ServerListeningService));
                m_ServerServiceIntent.PutExtra(ServerListeningService.ExtraIpAddress, hostAddress);
                m_ServerServiceIntent.PutExtra(ServerListeningService.ExtraListeningPort, port);
                context.StartService(m_ServerServiceIntent);

                IsListening = true;
            }
        }

        /// <summary>
        /// Stop listening to incoming client connections.
        /// </summary>
        /// <param name="context">The context.</param>
        public void StopListening(Context context)
        {
            if (IsListening)
            {
                context.StopService(m_ServerServiceIntent);

                IsListening = false;
            }
        }

        #endregion Methods
    }
}