using Android;

namespace iDatech.WifiP2p.Poc.Permissions
{
    /// <summary>
    /// Constants used for permission requests.
    /// </summary>
    static public class PermissionConstants
    {
        #region Constants

        /// <summary>
        /// <see cref="Manifest.Permission.AccessWifiState"/>
        /// <see cref="Manifest.Permission.ChangeWifiState"/>
        /// <see cref="Manifest.Permission.Internet"/>
        /// <see cref="Manifest.Permission.AccessFineLocation"/>
        /// <see cref="Manifest.Permission.AccessCoarseLocation"/>
        /// </summary>
        public const int WifiP2pPermissionsGroupId = 0;

        /// <summary>
        /// The permissions required to use Wifi P2P discovering.
        /// </summary>
        static public readonly string[] WifiP2pPermissionGroup = new[]
        {
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.Internet,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.AccessCoarseLocation
        };

        #endregion Constants
    }
}