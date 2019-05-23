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
        /// Called when data is being downloaded from a peer.
        /// </summary>
        /// <param name="progress">The current download progress.</param>
        void OnDataDownloadProgressChanged(int progress);

        /// <summary>
        /// Called when data download has finished.
        /// </summary>
        /// <param name="messageType">The type of message that was sent with the data. Can be used to cast the data accordingly.</param>
        /// <param name="data">The data that has been downloaded.</param>
        void OnDataDownloaded(EMessageType messageType, object data);

        /// <summary>
        /// Called when data is being sent to a peer.
        /// </summary>
        /// <param name="messageType">The type of message that was sent with the data. Can be used to cast the data accordingly.</param>
        /// <param name="progress">The current upload progress.</param>
        /// <param name="data">The data that is being sent.</param>
        void OnDataUploadProgressChanged(EMessageType messageType, int progress, object data);

        /// <summary>
        /// Called when data upload has finished.
        /// </summary>
        /// <param name="messageType">The type of message that was sent with the data. Can be used to cast the data accordingly.</param>
        /// <param name="data">The data that has been uploaded.</param>
        void OnDataUploaded(EMessageType messageType, object data);

        #endregion Methods
    }
}