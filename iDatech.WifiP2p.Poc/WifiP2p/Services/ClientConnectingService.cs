using Android.App;
using Android.Content;
using System;
using System.Net;
using System.Net.Sockets;

namespace iDatech.WifiP2p.Poc.WifiP2p.Services
{
    [Service]
    sealed public class ClientConnectingService : IntentService
    {
        #region Constants

        /// <summary>
        /// Extra parameter containing the host's IP address.
        /// </summary>
        public const string ExtraIpAddress = "EXTRA_IP_ADDRESS";

        /// <summary>
        /// Extra parameter containing the listening port.
        /// </summary>
        public const string ExtraListeningPort = "EXTRA_PORT";

        #endregion Constants

        #region Methods

        protected override void OnHandleIntent(Intent intent)
        {
            // Fetch the IP address and port from the intent extras.
            string targetAddress = intent.GetStringExtra(ExtraIpAddress) ?? throw new ArgumentException($"The intent did not contain the following parameter : {ExtraIpAddress}", nameof(intent));
            int port = intent.GetIntExtra(ExtraListeningPort, 863);
            if (!IPAddress.TryParse(targetAddress, out IPAddress ipAddress))
            {
                throw new ArgumentException($"The specified IP address {targetAddress} could not be parsed.", nameof(intent));
            }

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion Methods
    }
}