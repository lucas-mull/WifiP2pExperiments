using Android.Net.Wifi.P2p;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iDatech.WifiP2p.Poc
{
    /// <summary>
    /// Custom <see cref="RecyclerView.Adapter"/> used to display a list of WiFi P2P devices.
    /// </summary>
    sealed public class WifiPeerAdapter : RecyclerView.Adapter
    {
        #region Nested classes

        /// <summary>
        /// A custom <see cref="RecyclerView.ViewHolder"/> for <see cref="WifiPeerAdapter"/>.
        /// </summary>
        sealed private class WifiPeerViewHolder : RecyclerView.ViewHolder
        {
            #region Properties

            /// <summary>
            /// The image defining the type of device (tablet or phone)
            /// </summary>
            internal ImageView DeviceImage { get; }

            /// <summary>
            /// The text view displaying the device's name.
            /// </summary>
            internal TextView DeviceName { get; }

            /// <summary>
            /// The text view displaying the device address
            /// </summary>
            internal TextView DeviceAddress { get; }

            /// <summary>
            /// The text view displaying the device connection status.
            /// </summary>
            internal TextView DeviceStatus { get; }

            /// <summary>
            /// The button used to send data to a client.
            /// </summary>
            internal Button SendButton { get; }

            /// <summary>
            /// The container for the 3 text views.
            /// </summary>
            internal LinearLayout InformationLayout { get; set; }

            #endregion Properties

            #region Constructors

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="itemView">The item view representing the layout of the view holder.</param>
            internal WifiPeerViewHolder(View itemView) : base(itemView)
            {
                DeviceImage = itemView.FindViewById<ImageView>(Resource.Id.img_device);
                DeviceName = itemView.FindViewById<TextView>(Resource.Id.txt_device_name);
                DeviceAddress = itemView.FindViewById<TextView>(Resource.Id.txt_mac_address);
                DeviceStatus = itemView.FindViewById<TextView>(Resource.Id.txt_mac_address);
                SendButton = itemView.FindViewById<Button>(Resource.Id.button_send);
                InformationLayout = itemView.FindViewById<LinearLayout>(Resource.Id.info_layout);
            }

            #endregion Constructors
        }

        #endregion Nested classes

        #region Instance variables

        /// <summary>
        /// The list of devices to display.
        /// </summary>
        private HashSet<WifiP2pDevice> m_WifiP2pDevices;

        /// <summary>
        /// Action to execute when an item is clicked.
        /// </summary>
        readonly private Action<WifiP2pDevice> m_OnItemClick;

        /// <summary>
        /// Whether or not to show the button on the right of the line.
        /// </summary>
        private bool m_ShowButton;

        #endregion Instance variables

        #region Properties

        /// <summary>
        /// <see cref="RecyclerView.Adapter.ItemCount"/>
        /// </summary>
        override public int ItemCount => m_WifiP2pDevices.Count;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="wifiP2pDevices">The list of devices used to populate the recycler view.</param>
        /// <param name="onItemClick">Action to execute when an item is clicked.</param>
        public WifiPeerAdapter(HashSet<WifiP2pDevice> wifiP2pDevices, bool showButton = true, Action<WifiP2pDevice> onItemClick = null)
        {
            m_WifiP2pDevices = wifiP2pDevices ?? throw new ArgumentNullException(nameof(wifiP2pDevices));
            m_ShowButton = showButton;
            m_OnItemClick = onItemClick;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="RecyclerView.Adapter.OnBindViewHolder(RecyclerView.ViewHolder, int)"/>
        /// </summary>
        override public void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            WifiPeerViewHolder wifiPeerHolder = holder as WifiPeerViewHolder;
            WifiP2pDevice currentItem = m_WifiP2pDevices.ElementAt(position);

            wifiPeerHolder.DeviceImage.SetImageResource(position % 2 == 0 ? Resource.Drawable.ic_tablet_android : Resource.Drawable.ic_phone_android);
            wifiPeerHolder.DeviceName.Text = currentItem.DeviceName;
            wifiPeerHolder.InformationLayout.SetOnClickListener(new BaseOnClickListener(v =>
            {
                m_OnItemClick?.Invoke(currentItem);
            }));

            wifiPeerHolder.SendButton.Visibility = m_ShowButton ? ViewStates.Visible : ViewStates.Gone;
        }

        /// <summary>
        /// <see cref="RecyclerView.Adapter.OnCreateViewHolder(ViewGroup, int)"/>
        /// </summary>
        override public RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.wifi_device_line, parent, false);

            return new WifiPeerViewHolder(itemView);
        }

        /// <summary>
        /// Update the list of devices with a new one and refresh the view.
        /// </summary>
        /// <param name="wifiP2pDevices">The new list of devices.</param>
        public void UpdateDataset(HashSet<WifiP2pDevice> wifiP2pDevices)
        {
            m_WifiP2pDevices = wifiP2pDevices ?? throw new ArgumentNullException(nameof(wifiP2pDevices));

            NotifyDataSetChanged();
        }

        #endregion Methods
    }
}