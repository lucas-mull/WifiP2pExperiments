using Android.Content;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Services;
using System;
using System.Collections.Generic;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// The singleton, client-side, used to hold all the data that has been sent to / received by the clients
    /// </summary>
    sealed public class Client
    {
        #region Static variables

        /// <summary>
        /// The singleton instance.
        /// </summary>
        static public Client Instance { get; }

        #endregion Static variables

        #region Static constructor

        /// <summary>
        /// Initialize the only instance in the static constructor.
        /// </summary>
        static Client()
        {
            Instance = new Client();
        }

        #endregion Static constructor

        #region Instance variables

        /// <summary>
        /// The intent used to start the <see cref="ClientConnectingService"/>.
        /// </summary>
        private Intent m_ClientServiceIntent;

        #endregion Instance variables

        #region Properties

        /// <summary>
        /// The exhaustive list of all the chat messages that were sent.
        /// </summary>
        public List<ChatMessage> SentMessages { get; private set; }

        /// <summary>
        /// The exhaustive list of all the chat messages that were received.
        /// </summary>
        public List<ChatMessage> ReceivedMessages { get; private set; }

        /// <summary>
        /// The list of pending messages (messages that need to be sent but haven't been yet).
        /// </summary>
        public Queue<Message> PendingMessages { get; private set; }

        /// <summary>
        /// The info of the access point we are connected to.
        /// </summary>
        public DeviceInfo AccessPoint { get; private set; }

        /// <summary>
        /// The info about the current device.
        /// </summary>
        public DeviceInfo DeviceInfo { get; private set; }

        /// <summary>
        /// Whether or not the client is currently connected to the server.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Has the client pinged the server with their personal info ?
        /// </summary>
        public bool HasPinged { get; private set; }

        /// <summary>
        /// Is the <see cref="ClientConnectingService"/> currently started ?
        /// </summary>
        public bool IsSending { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Private constructor. Initialize all the collections.
        /// </summary>
        private Client()
        {
            SentMessages = new List<ChatMessage>();
            ReceivedMessages = new List<ChatMessage>();
            PendingMessages = new Queue<Message>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Push a message to be sent.
        /// </summary>
        /// <param name="message">The message.</param>
        public void PushMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            PendingMessages.Enqueue(message);
        }

        /// <summary>
        /// Set the access point this device is currently connected to.
        /// </summary>
        /// <param name="accessPoint">The access point info.</param>
        public void SetConnected(DeviceInfo accessPoint)
        {
            AccessPoint = accessPoint;
        }

        /// <summary>
        /// Start the background service that sends pending messages to the server periodically.
        /// </summary>
        /// <param name="context">The context(used to start the sending service).</param>
        /// <param name="serverAddress">The server's address.</param>
        /// <param name="port">The port to connect to.</param>
        public void StartSending(Context context, string serverAddress, int port)
        {
            if (!IsSending)
            {
                m_ClientServiceIntent = new Intent(context, typeof(ClientConnectingService));
                m_ClientServiceIntent.PutExtra(ClientConnectingService.ExtraIpAddress, serverAddress);
                m_ClientServiceIntent.PutExtra(ClientConnectingService.ExtraConnectingPort, port);

                context.StartService(m_ClientServiceIntent);
                IsSending = true;
            }
        }

        /// <summary>
        /// Stop listening to incoming client connections.
        /// </summary>
        /// <param name="context">The context.</param>
        public void StopSending(Context context)
        {
            if (IsSending)
            {
                context.StopService(m_ClientServiceIntent);

                IsSending = false;
            }
        }

        /// <summary>
        /// Remove the access point.
        /// </summary>
        public void Disconnect()
        {
            AccessPoint = null;
        }

        #endregion Methods
    }
}