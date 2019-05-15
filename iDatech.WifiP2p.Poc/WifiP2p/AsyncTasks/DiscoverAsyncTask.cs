using Android.App;
using Android.Net.Wifi.P2p;
using Android.OS;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using System;
using System.Diagnostics;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    /// <summary>
    /// Async task used to discover peers and stop after a specified time.
    /// </summary>
    public class DiscoverAsyncTask : AsyncTask
    {
        #region Events

        /// <summary>
        /// Event fired when the peer discovery has been stopped.
        /// </summary>
        public event Action DiscoveryStopped;

        #endregion Events

        #region Instance variables

        /// <summary>
        /// The time out value (in seconds).
        /// </summary>
        private readonly float m_TimeOutSeconds;

        /// <summary>
        /// The calling activity.
        /// </summary>
        private readonly Activity m_Activity;

        /// <summary>
        /// The <see cref="WifiP2pManager"/> instance.
        /// </summary>
        private readonly WifiP2pManager m_WifiP2pManager;

        /// <summary>
        /// The channel used for Wifi P2P communications.
        /// </summary>
        private readonly WifiP2pManager.Channel m_WifiP2pChannel;

        /// <summary>
        /// Watch used to compute elapsed time.
        /// </summary>
        private Stopwatch m_Watch;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Specify the timeOut in milli seconds.
        /// </summary>
        /// <param name="activity">The context.</param>
        /// <param name="manager">The wifi P2P manager.</param>
        /// <param name="channel">The channel used for wifi P2P communications.</param>
        /// <param name="timeOutSeconds">How long the task should keep discovering peers. Use a zero or a negative value for indefinite time.</param>
        /// <remarks>For indefinite times, the method <see cref="WifiP2pManager.StopPeerDiscovery(WifiP2pManager.Channel, WifiP2pManager.IActionListener)"/> must be called manually.</remarks>
        public DiscoverAsyncTask(Activity activity, WifiP2pManager manager, WifiP2pManager.Channel channel, float timeOutSeconds)
        {
            m_Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            m_WifiP2pManager = manager ?? throw new ArgumentNullException(nameof(manager));
            m_WifiP2pChannel = channel ?? throw new ArgumentNullException(nameof(channel));
            m_TimeOutSeconds = timeOutSeconds;
        }

        #endregion Constructors

        /// <summary>
        /// <see cref="AsyncTask.OnPreExecute"/>
        /// </summary>
        override protected void OnPreExecute()
        {
            base.OnPreExecute();

            if (m_TimeOutSeconds > 0)
            {
                m_Watch = Stopwatch.StartNew();
            }

            m_WifiP2pManager.DiscoverPeers(m_WifiP2pChannel, new WifiP2pActionListener(m_Activity, EWifiP2pAction.Discover));
        }

        /// <summary>
        /// <see cref="AsyncTask.DoInBackground(Java.Lang.Object[])"/>
        /// </summary>
        override protected Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (m_TimeOutSeconds <= 0)
            {
                return null;
            }

            while (true)
            {
                if (m_Watch.ElapsedMilliseconds >= m_TimeOutSeconds * 1000)
                {
                    //m_WifiP2pManager.StopPeerDiscovery(m_WifiP2pChannel, new WifiP2pActionListener(m_Activity, EWifiP2pAction.StopDiscovery));
                    return null;
                }
            }
        }

        /// <summary>
        /// <see cref="AsyncTask.OnPostExecute(Java.Lang.Object)"/>
        /// </summary>
        override protected void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);

            if (m_TimeOutSeconds > 0)
            {
                DiscoveryStopped?.Invoke();
            }
        }
    }
}