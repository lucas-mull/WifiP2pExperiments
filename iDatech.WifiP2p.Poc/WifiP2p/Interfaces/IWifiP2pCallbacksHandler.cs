using Android.Net;
using Android.Net.Wifi.P2p;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;

namespace iDatech.WifiP2p.Poc.WifiP2p.Interfaces
{
    /// <summary>
    /// Global interface used to gather all Wifi P2P related callbacks.
    /// </summary>
    public interface IWifiP2pCallbacksHandler
    {
        #region Methods

        /// <summary>
        /// Called whenever a P2P connection has changed (connected / disconnected).
        /// </summary>
        /// <param name="networkInfo">The network information.</param>
        /// <param name="p2pInfo">The P2P information.</param>
        /// <param name="groupInfo">The P2P group information (if one was formed).</param>
        void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo);

        /// <summary>
        /// Called whenever Wifi P2P is enabled / disabled.
        /// </summary>
        /// <param name="newState">The new state.</param>
        void OnWifiP2pStateChanged(EWifiState newState);

        /// <summary>
        /// Called whenever the current device details have changed.
        /// </summary>
        /// <param name="deviceDetails">The new details.</param>
        void OnThisDeviceChanged(WifiP2pDevice deviceDetails);

        /// <summary>
        /// Called whenever the list of peers has been updated.
        /// </summary>
        /// <param name="peers">The current list of peers.</param>
        void OnPeersAvailable(WifiP2pDeviceList peers);

        #endregion Methods
    }
}