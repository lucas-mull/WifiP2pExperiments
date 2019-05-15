using Android.Support.V7.Widget;
using Android.Widget;

namespace iDatech.WifiP2p.Poc.Activities.Views
{
    /// <summary>
    /// Simple view holder for <see cref="AccessPointActivity"/>
    /// </summary>
    sealed public class AccessPointViews
    {
        #region Properties

        public TextView SsidTextView { get; private set; }
        public TextView DeviceNameTextView { get; private set; }
        public TextView DeviceAddressTextView { get; private set; }
        public TextView DeviceStatusTextView { get; private set; }
        public ImageView DeviceTypeImageView { get; private set; }
        public Button DetailsButton { get; private set; }
        public Button DisconnectButton { get; private set; }
        public RelativeLayout DetailsLayout { get; private set; }
        public RecyclerView MembersRecyclerView { get; private set; }

        #endregion Properties

        #region Constructors

        public AccessPointViews(AccessPointActivity parent)
        {
            SsidTextView = parent.FindViewById<TextView>(Resource.Id.txt_ssid);
            DeviceNameTextView = parent.FindViewById<TextView>(Resource.Id.txt_device_name);
            DeviceAddressTextView = parent.FindViewById<TextView>(Resource.Id.txt_mac_address);
            DeviceStatusTextView = parent.FindViewById<TextView>(Resource.Id.txt_status);
            DeviceTypeImageView = parent.FindViewById<ImageView>(Resource.Id.img_device_type);
            DetailsButton = parent.FindViewById<Button>(Resource.Id.button_details);
            DisconnectButton = parent.FindViewById<Button>(Resource.Id.button_remove_group);
            DetailsLayout = parent.FindViewById<RelativeLayout>(Resource.Id.layout_details);
            MembersRecyclerView = parent.FindViewById<RecyclerView>(Resource.Id.recyclerview_group_members);
            MembersRecyclerView.SetLayoutManager(new LinearLayoutManager(parent));
        }

        #endregion Constructors
    }
}