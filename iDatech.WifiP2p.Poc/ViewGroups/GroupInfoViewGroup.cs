using Android.Support.V7.Widget;
using Android.Widget;

namespace iDatech.WifiP2p.Poc.ViewGroups
{
    /// <summary>
    /// Simple wrapper for the view holding the wifi P2P group information.
    /// </summary>
    internal class GroupInfoViewGroup
    {
        #region Properties

        /// <summary>
        /// Used to display the network SSID.
        /// </summary>
        internal TextView SsidTextView { get; set; }

        /// <summary>
        /// Used to display the group passphrase.
        /// </summary>
        internal TextView PassphraseTextView { get; set; }

        /// <summary>
        /// The recycler view holding the group members.
        /// </summary>
        internal RecyclerView MembersRecyclerView { get; set; }

        #endregion Properties
    }
}