using Android.Net.Wifi.P2p;

namespace iDatech.WifiP2p.Poc.WifiP2p.Enums
{
    /// <summary>
    /// List of all the actions related to <see cref="WifiP2pManager.IActionListener"/>
    /// </summary>
    public enum EWifiP2pAction
    {
        Discover,
        StopDiscovery,
        CreateGroup,
        EndGroup,
        Connect,
        Disconnect
    }
}