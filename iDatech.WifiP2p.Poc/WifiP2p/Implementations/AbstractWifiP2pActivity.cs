using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Support.V7.App;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// Base class for an activity that receives Wifi P2P related callbacks.
    /// </summary>
    abstract public class AbstractWifiP2pActivity : AppCompatActivity, IWifiP2pCallbacksHandler
    {
        #region Instance variables

        /// <summary>
        /// Receiver for Wifi P2P broadcasts.
        /// </summary>
        private WifiP2pBroadcastReceiver m_WifiP2pBroadcastReceiver;

        /// <summary>
        /// The intent filter for WifiP2p actions.
        /// </summary>
        private IntentFilter m_WifiP2pIntentFilter;

        #endregion Instance variables

        #region Properties

        /// <summary>
        /// The <see cref="WifiP2pManager"/> instance.
        /// </summary>
        protected WifiP2pManager WifiP2pManager { get; set; }

        /// <summary>
        /// The main channel used for Wifi P2P communications on this device.
        /// </summary>
        protected WifiP2pManager.Channel WifiP2pChannel { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// <see cref="Activity.OnCreate(Bundle)"/>
        /// </summary>
        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            WifiP2pManager = (WifiP2pManager)GetSystemService(WifiP2pService);
            WifiP2pChannel = WifiP2pManager.Initialize(this, MainLooper, null);
            m_WifiP2pBroadcastReceiver = new WifiP2pBroadcastReceiver(WifiP2pManager, WifiP2pChannel, this);

            m_WifiP2pIntentFilter = new IntentFilter();
            m_WifiP2pIntentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
            m_WifiP2pIntentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
            m_WifiP2pIntentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
            m_WifiP2pIntentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);
        }

        /// <summary>
        /// <see cref="Activity.OnResume()"/>
        /// </summary>
        override protected void OnResume()
        {
            base.OnResume();
            RegisterReceiver(m_WifiP2pBroadcastReceiver, m_WifiP2pIntentFilter);
        }

        /// <summary>
        /// <see cref="Activity.OnPause()"/>
        /// </summary>
        override protected void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(m_WifiP2pBroadcastReceiver);
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnPeersAvailable(WifiP2pDeviceList)"/>
        /// </summary>
        abstract public void OnPeersAvailable(WifiP2pDeviceList peers);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        abstract public void OnThisDeviceChanged(WifiP2pDevice deviceDetails);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        abstract public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        abstract public void OnWifiP2pStateChanged(EWifiState newState);

        #endregion Methods
    }
}