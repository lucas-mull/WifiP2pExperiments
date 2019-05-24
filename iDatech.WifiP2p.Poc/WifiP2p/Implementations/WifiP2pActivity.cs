using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Support.V7.App;
using iDatech.WifiP2p.Poc.Parcelable;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using static Android.Net.Wifi.P2p.WifiP2pManager;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// Base class for an activity that receives Wifi P2P related callbacks.
    /// </summary>
    abstract public class WifiP2pActivity : AppCompatActivity, IWifiP2pCallbacksHandler, IGroupInfoListener, IPeerListListener, IConnectionInfoListener
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
        protected Channel WifiP2pChannel { get; set; }

        /// <summary>
        /// Is this device the access point ?
        /// </summary>
        protected bool IsServer { get; set; }

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
        /// Request an updated peer list explicitly.
        /// </summary>
        /// <remarks>Can return an empty list if no peers have been discovered.
        /// Catch the results with the method <see cref="OnPeersAvailable(WifiP2pDeviceList)"/></remarks>
        protected void RequestUpdatedPeerList()
        {
            WifiP2pManager.RequestPeers(WifiP2pChannel, this);
        }

        /// <summary>
        /// Request for updated information about the current group.
        /// </summary>
        /// <remarks>Only the group owner can obtain the extended info about the group. Clients can only receive the owner device info.
        /// Catch the results with the method <see cref="OnGroupInfoAvailable(WifiP2pGroup)"/></remarks>
        protected void RequestGroupInfo()
        {
            WifiP2pManager.RequestGroupInfo(WifiP2pChannel, this);
        }

        /// <summary>
        /// Request for updated information about the current connection i.e. if a group has been formed, the group owner inet address and whether or not this device is the group owner.
        /// </summary>
        /// <remarks>Catch the results with the method <see cref="OnConnectionInfoAvailable(WifiP2pInfo)"/></remarks>
        protected void RequestConnectionInfo()
        {
            WifiP2pManager.RequestConnectionInfo(WifiP2pChannel, this);
        }

        /// <summary>
        /// <see cref="IPeerListListener.OnPeersAvailable(WifiP2pDeviceList)"/>
        /// </summary>
        virtual public void OnPeersAvailable(WifiP2pDeviceList peers) { }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        virtual public void OnThisDeviceChanged(WifiP2pDevice deviceDetails) { }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        virtual public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo)
        {
            if (networkInfo.IsConnected && p2pInfo.GroupFormed)
            {
                if (p2pInfo.IsGroupOwner)
                {
                    IsServer = true;
                    AccessPoint.Instance.StartListening(this, p2pInfo.GroupOwnerAddress.HostAddress, 8898);
                    foreach(WifiP2pDevice device in groupInfo.ClientList)
                    {
                        AccessPoint.Instance.TryAddClient(new DeviceInfo(device.DeviceName, device.DeviceAddress, p2pInfo.GroupOwnerAddress.HostAddress));
                    }
                }
                else
                {
                    IsServer = false;
                    Client.Instance.SetConnected(new DeviceInfo(groupInfo.Owner.DeviceName, groupInfo.Owner.DeviceAddress, p2pInfo.GroupOwnerAddress.HostAddress));
                    Client.Instance.StartSending(this, p2pInfo.GroupOwnerAddress.HostAddress, 8898);
                }
            }
            else
            {
                if (IsServer)
                {
                    AccessPoint.Instance.StopListening(this);
                }
                else
                {
                    Client.Instance.StopSending(this);
                }
            }
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        virtual public void OnWifiP2pStateChanged(EWifiState newState) { }

        /// <summary>
        /// <see cref="IGroupInfoListener.OnGroupInfoAvailable(WifiP2pGroup)"/>
        /// </summary>
        virtual public void OnGroupInfoAvailable(WifiP2pGroup group) { }

        /// <summary>
        /// <see cref="IConnectionInfoListener.OnConnectionInfoAvailable(WifiP2pInfo)"/>
        /// </summary>
        virtual public void OnConnectionInfoAvailable(WifiP2pInfo info) { }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnMessageReceivedProgressChanged(EMessageType, int)"/>
        /// </summary>
        abstract public void OnMessageReceivedProgressChanged(EMessageType message, float progress);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnMessageReceived(Message)"/>
        /// </summary>
        abstract public void OnMessageReceived(Message message);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnMessageSendingProgressChanged(Message, float)"/>
        /// </summary>
        abstract public void OnMessageSendingProgressChanged(Message message, float progress);

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnMessageSent(Message)"/>
        /// </summary>
        abstract public void OnMessageSent(Message message);

        #endregion Methods
    }
}