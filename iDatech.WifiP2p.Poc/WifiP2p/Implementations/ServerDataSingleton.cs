using iDatech.WifiP2p.Poc.Parcelable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// The singleton, server-side, used to hold all the data that has been sent to / received by the clients
    /// </summary>
    sealed public class ServerDataSingleton
    {
        #region Static variables

        /// <summary>
        /// The singleton instance.
        /// </summary>
        static public ServerDataSingleton Instance { get; }

        #endregion Static variables

        #region Static constructor

        /// <summary>
        /// Initialize the only instance in the static constructor.
        /// </summary>
        static ServerDataSingleton()
        {
            Instance = new ServerDataSingleton();
        }

        #endregion Static constructor

        #region Properties

        /// <summary>
        /// The exhaustive list of all the chat messages that were received.
        /// </summary>
        public List<ChatMessage> Messages { get; private set; }

        /// <summary>
        /// The list of pending messages, for each device (uses the mac address as key).
        /// </summary>
        public Dictionary<string, Stack<ChatMessage>> PendingMessages { get; private set; }

        /// <summary>
        /// The list of currently connected clients. Stores the mac address since it's unique to each client.
        /// </summary>
        public List<string> ConnectedClients { get; private set; }

        /// <summary>
        /// Whether or not the server is currently listening to incoming connections.
        /// </summary>
        public bool IsListening { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Private constructor. Initialize all the collections.
        /// </summary>
        private ServerDataSingleton()
        {
            Messages = new List<ChatMessage>();
            PendingMessages = new Dictionary<string, Stack<ChatMessage>>();
            ConnectedClients = new List<string>();
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
                        PendingMessages[address].Push(message);
                    }
                }
            }
        }

        /// <summary>
        /// try to add a new client's mac address.
        /// </summary>
        /// <param name="clientAddress">The new client's address.</param>
        /// <returns><c>true</c> if the client was added successfully, <c>false</c> if it was already registered.</returns>
        public bool TryAddClient(string clientAddress)
        {
            if (ConnectedClients.Contains(clientAddress))
            {
                return false;
            }

            lock (ConnectedClients)
            {
                ConnectedClients.Add(clientAddress);
            }

            // Initialize the pending messages for this client
            if (!PendingMessages.ContainsKey(clientAddress))
            {
                lock (PendingMessages)
                {
                    PendingMessages.Add(clientAddress, new Stack<ChatMessage>());
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
                ConnectedClients.Remove(clientAddress);
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
        /// Set the server listening state.
        /// </summary>
        /// <param name="isListening"><c>true</c> if listening, <c>false</c> otherwise.</param>
        public void SetListening(bool isListening)
        {
            IsListening = false;
        }

        #endregion Methods
    }
}