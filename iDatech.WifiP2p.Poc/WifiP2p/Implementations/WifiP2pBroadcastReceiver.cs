using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using System;
using static Android.Net.Wifi.P2p.WifiP2pManager;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// Broadcast receiver for Wifi P2p events.
    /// </summary>
    sealed public class WifiP2pBroadcastReceiver : BroadcastReceiver
    {
        #region Instance variables

        /// <summary>
        /// The <see cref="WifiP2pManager"/> instance.
        /// </summary>
        readonly private WifiP2pManager m_WifiP2pManager;

        /// <summary>
        /// The requested <see cref="Channel"/>.
        /// </summary>
        readonly private Channel m_Channel;

        /// <summary>
        /// The handler for all the callbacks.
        /// </summary>
        readonly private AbstractWifiP2pActivity m_CallbackHandler;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="wifiP2pManager">The <see cref="WifiP2pManager"/> instance.</param>
        /// <param name="channel">The used channel.</param>
        /// <param name="callbackHandler">The global callback handler for wifi P2P signals.</param>
        public WifiP2pBroadcastReceiver(WifiP2pManager wifiP2pManager, Channel channel, AbstractWifiP2pActivity callbackHandler)
        {
            m_WifiP2pManager = wifiP2pManager ?? throw new ArgumentNullException(nameof(wifiP2pManager));
            m_Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            m_CallbackHandler = callbackHandler ?? throw new ArgumentNullException(nameof(callbackHandler));
        }

        #endregion Constructors

        /// <summary>
        /// <see cref="BroadcastReceiver.OnReceive(Context, Intent)"/>
        /// </summary>
        override public void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case WifiP2pConnectionChangedAction:
                    NetworkInfo networkInfo = (NetworkInfo)intent.GetParcelableExtra(ExtraNetworkInfo);
                    WifiP2pInfo p2pInfo = (WifiP2pInfo)intent.GetParcelableExtra(ExtraWifiP2pInfo);
                    WifiP2pGroup groupInfo = (WifiP2pGroup)intent.GetParcelableExtra(ExtraWifiP2pGroup);
                    m_CallbackHandler.OnWifiP2pConnectionChanged(networkInfo, p2pInfo, groupInfo);
                    break;

                case WifiP2pPeersChangedAction:
                    WifiP2pDeviceList peers = (WifiP2pDeviceList)intent.GetParcelableExtra(ExtraP2pDeviceList);
                    m_CallbackHandler.OnPeersAvailable(peers);
                    break;

                case WifiP2pStateChangedAction:
                    EWifiState wifiState = (EWifiState)intent.GetIntExtra(ExtraWifiState, 0);
                    m_CallbackHandler.OnWifiP2pStateChanged(wifiState);
                    break;

                case WifiP2pThisDeviceChangedAction:
                    WifiP2pDevice deviceDetails = (WifiP2pDevice)intent.GetParcelableExtra(ExtraWifiP2pDevice);
                    m_CallbackHandler.OnThisDeviceChanged(deviceDetails);
                    break;

                default:
                    throw new NotSupportedException($"The action {intent.Action} is not handled by this receiver.");
            }
        }
    }
}