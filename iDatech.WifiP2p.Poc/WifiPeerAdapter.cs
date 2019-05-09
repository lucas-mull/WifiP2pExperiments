using Android.Net.Wifi.P2p;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

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
            /// The text view display the device's name.
            /// </summary>
            internal TextView DeviceName { get; }

            /// <summary>
            /// A button to access the current device's settings.
            /// </summary>
            internal ImageButton DeviceSettingsButton { get; }

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
                DeviceSettingsButton = itemView.FindViewById<ImageButton>(Resource.Id.btn_device_settings);
            }

            #endregion Constructors
        }

        #endregion Nested classes

        #region Instance variables

        /// <summary>
        /// The list of devices to display.
        /// </summary>
        private IList<WifiP2pDevice> m_WifiP2pDevices;

        /// <summary>
        /// Action to execute when an item is clicked.
        /// </summary>
        private Action<WifiP2pDevice> m_OnItemClick;

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
        public WifiPeerAdapter(IList<WifiP2pDevice> wifiP2pDevices, Action<WifiP2pDevice> onItemClick = null)
        {
            m_WifiP2pDevices = wifiP2pDevices ?? throw new ArgumentNullException(nameof(wifiP2pDevices));
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
            WifiP2pDevice currentItem = m_WifiP2pDevices[position];

            wifiPeerHolder.DeviceImage.SetImageResource(position % 2 == 0 ? Resource.Drawable.ic_tablet_android : Resource.Drawable.ic_phone_android);
            wifiPeerHolder.DeviceName.Text = currentItem.DeviceName;
            wifiPeerHolder.DeviceName.SetOnClickListener(new BaseOnClickListener(v =>
            {
                m_OnItemClick?.Invoke(currentItem);
            }));
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
        public void UpdateDataset(IList<WifiP2pDevice> wifiP2pDevices)
        {
            m_WifiP2pDevices = wifiP2pDevices ?? throw new ArgumentNullException(nameof(wifiP2pDevices));

            NotifyDataSetChanged();
        }

        #endregion Methods
    }
}