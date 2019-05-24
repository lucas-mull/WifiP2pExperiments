using Android.App;
using Android.Content;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace iDatech.WifiP2p.Poc.WifiP2p.Services
{
    /// <summary>
    /// The background service responsible for connecting to the server periodically and sending the needed messages.
    /// </summary>
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
        public const string ExtraConnectingPort = "EXTRA_PORT";

        /// <summary>
        /// Connection attempts rate in seconds.
        /// Every x seconds a connection will be attempted (if need be).
        /// </summary>
        private const int LoopRateinSeconds = 10;

        #endregion Constants

        #region Methods

        /// <summary>
        /// <see cref="IntentService.OnHandleIntent(Intent)"/>
        /// </summary>
        override protected void OnHandleIntent(Intent intent)
        {
            // Only allow one instance of the service to be running at once.
            if (Client.Instance.IsSending)
            {
                StopSelf();
            }
            else
            {
                DoBackgroundWork(intent);
            }
        }

        /// <summary>
        /// Do the background work.
        /// </summary>
        /// <param name="intent">The intent passed on from <see cref="OnHandleIntent(Intent)"/></param>
        private void DoBackgroundWork(Intent intent)
        {
            // Fetch the socket parameters from the intent
            string targetAddress = intent.GetStringExtra(ExtraIpAddress) ?? throw new ArgumentException($"The intent did not contain the following parameter : {ExtraIpAddress}", nameof(intent));
            int port = intent.GetIntExtra(ExtraConnectingPort, 863);
            if (!IPAddress.TryParse(targetAddress, out IPAddress ipAddress))
            {
                throw new ArgumentException($"The specified IP address {targetAddress} could not be parsed.", nameof(intent));
            }

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Fetch the IP address and port from the intent extras.
            while (true)
            {
                Thread.Sleep(LoopRateinSeconds);

                if (Client.Instance.PendingMessages.Any())
                {
                    Message toSend = Client.Instance.PendingMessages.Dequeue();
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    sender.Connect(remoteEP);

                    toSend.Send(sender, this);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }
        }

        #endregion Methods
    }
}