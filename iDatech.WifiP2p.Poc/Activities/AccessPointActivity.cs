
using Android.App;
using Android.Net;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Views;
using Android.Widget;
using iDatech.WifiP2p.Poc.Activities.Views;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using System.Linq;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "AccessPointActivity")]
    public class AccessPointActivity : AbstractWifiP2pActivity
    {
        #region Instance variables

        /// <summary>
        /// The useful views in this layout.
        /// </summary>
        private AccessPointViews m_Views;

        /// <summary>
        /// Keep track of the details layout visibility state.
        /// </summary>
        private bool m_AreDetailsShown;

        #endregion Instance variables

        #region Methods

        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_access_point);

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            m_Views = new AccessPointViews(this);

            m_Views.DetailsLayout.Visibility = Android.Views.ViewStates.Gone;

            WifiP2pManager.CreateGroup(WifiP2pChannel, new WifiP2pActionListener(this, EWifiP2pAction.CreateGroup, () =>
            {
                Toast.MakeText(this, "Groupe créé avec succès !", ToastLength.Short).Show();
            }));

            FindViewById<ViewGroup>(Resource.Id.layout_root).LayoutTransition.EnableTransitionType(Android.Animation.LayoutTransitionType.Changing);

            InitializeButtons();
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnPeersAvailable(WifiP2pDeviceList)"/>
        /// </summary>
        override public void OnPeersAvailable(WifiP2pDeviceList peers)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        override public void OnThisDeviceChanged(WifiP2pDevice deviceDetails)
        {
            m_Views.DeviceNameTextView.Text = deviceDetails.DeviceName;
            m_Views.DeviceAddressTextView.Text = deviceDetails.DeviceAddress;
            m_Views.DeviceStatusTextView.Text = deviceDetails.Status.ToString();
            //m_Views.DeviceTypeImageView.s
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        override public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo)
        {
            m_Views.SsidTextView.Text = groupInfo.NetworkName;

            m_Views.MembersRecyclerView.SetAdapter(new WifiPeerAdapter(groupInfo.ClientList.ToList()));
            m_Views.MembersRecyclerView.GetAdapter().NotifyDataSetChanged();
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        override public void OnWifiP2pStateChanged(EWifiState newState)
        {
            //throw new System.NotImplementedException();
            // Do nothing
        }

        private void InitializeButtons()
        {
            m_Views.DetailsButton.Click += (e, args) =>
            {
                m_Views.DetailsLayout.Visibility = m_AreDetailsShown ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;
                m_Views.DetailsButton.Text = m_AreDetailsShown ? "Détails" : "Cacher les détails";
                m_AreDetailsShown = !m_AreDetailsShown;
            };

            m_Views.DisconnectButton.Click += (e, args) =>
            {
                WifiP2pManager.RemoveGroup(WifiP2pChannel, new WifiP2pActionListener(this, EWifiP2pAction.EndGroup));
                OnBackPressed();
            };
        }

        #endregion Methods
    }
}