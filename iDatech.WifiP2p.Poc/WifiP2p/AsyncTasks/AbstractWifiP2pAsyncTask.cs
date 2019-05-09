using Android.App;
using Android.Net.Wifi.P2p;
using Android.OS;
using System;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    /// <summary>
    /// Base class for all wifi P2P related async tasks.
    /// </summary>
    abstract public class AbstractWifiP2pAsyncTask : AsyncTask
    {
        #region Instance variables

        /// <summary>
        /// The application activity - needed to update UI for instance.
        /// </summary>
        readonly protected Activity m_Activity;

        /// <summary>
        /// The connection information. Used to fetch the server (group owner) 's address.
        /// </summary>
        readonly protected WifiP2pInfo m_ConnectionInfo;

        /// <summary>
        /// Communication port.
        /// </summary>
        readonly protected int m_Port;

        /// <summary>
        /// Requests timeout.
        /// </summary>
        readonly protected int m_RequestTimeout;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor passing in the relevant connection information.
        /// </summary>
        /// <param name="activity">The application activity - needed to update UI for instance.</param>
        /// <param name="connectionInfo">The Wifi P2P connection info.</param>
        /// <param name="port">The communication port.</param>
        /// <param name="requestTimeout">The time after which a request times out.</param>
        protected AbstractWifiP2pAsyncTask(Activity activity, WifiP2pInfo connectionInfo, int port, int requestTimeout)
        {
            m_Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            m_ConnectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));
            m_Port = port >= 1 ? port : throw new ArgumentException("Negative values are not valid port numbers", nameof(port));
            m_RequestTimeout = requestTimeout;
        }

        #endregion Constructors
    }
}