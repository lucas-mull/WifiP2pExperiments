using Android.Widget;

namespace iDatech.WifiP2p.Poc.ViewGroups
{
    /// <summary>
    /// Simple wrapper for the view holding the connection information.
    /// </summary>
    internal class ConnectionInfoViewGroup
    {
        #region Properties

        /// <summary>
        /// Used to display connection status (yes / no)
        /// </summary>
        internal TextView StatusTextView { get; set; }

        /// <summary>
        /// Used to display type (client / server)
        /// </summary>
        internal TextView TypeTextView { get; set; }

        #endregion Properties
    }
}