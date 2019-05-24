
using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iDatech.WifiP2p.Poc.Activities.Views;
using iDatech.WifiP2p.Poc.Permissions;
using iDatech.WifiP2p.Poc.WifiP2p;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using System.Collections.Generic;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "AccessPointActivity")]
    public class AccessPointActivity : WifiP2pActivity
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

        /// <summary>
        /// The adapter for the recycler view.
        /// </summary>
        private WifiPeerAdapter m_Adapter;

        /// <summary>
        /// The list of connected clients.
        /// </summary>
        private HashSet<WifiP2pDevice> m_Clients;

        /// <summary>
        /// The permission service.
        /// </summary>
        private IPermissionService m_PermissionService;

        #endregion Instance variables

        #region Methods

        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_access_point);

            m_Views = new AccessPointViews(this);

            m_Views.DetailsLayout.Visibility = ViewStates.Gone;

            m_PermissionService = new PermissionService(this);

            m_Clients = new HashSet<WifiP2pDevice>();
            m_Adapter = new WifiPeerAdapter(m_Clients);
            m_Views.MembersRecyclerView.SetAdapter(m_Adapter);

            FindViewById<ViewGroup>(Resource.Id.layout_root).LayoutTransition.EnableTransitionType(Android.Animation.LayoutTransitionType.Changing);

            InitializeButtons();

            if (!m_PermissionService.IsPermissionGroupGranted(PermissionConstants.WifiP2pPermissionGroup))
            {
                m_PermissionService.RequestPermissions(PermissionConstants.WifiP2pPermissionsGroupId, PermissionConstants.WifiP2pPermissionGroup);
            }
            else
            {
                WifiP2pManager.CreateGroup(WifiP2pChannel, new WifiP2pActionListener(this, EWifiP2pAction.CreateGroup, () =>
                {
                    Toast.MakeText(this, "Groupe créé avec succès !", ToastLength.Short).Show();
                }));
            }
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        override public void OnThisDeviceChanged(WifiP2pDevice deviceDetails)
        {
            // Update the device's details
            m_Views.DeviceNameTextView.Text = deviceDetails.DeviceName;
            m_Views.DeviceAddressTextView.Text = deviceDetails.DeviceAddress;
            m_Views.DeviceStatusTextView.Text = deviceDetails.Status.ToString();
        }

        /// <summary>
        /// <see cref="Activity.OnRequestPermissionsResult(int, string[], Permission[])"/>
        /// </summary>
        override public void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case PermissionConstants.WifiP2pPermissionsGroupId:
                    bool allGranted = true;
                    for (int i = 0; i < permissions.Length; i++)
                    {
                        if (grantResults[i] != Permission.Granted)
                        {
                            allGranted = false;
                            break;
                        }
                    }

                    if (!allGranted)
                    {
                        Toast.MakeText(this, $"Permissions refusées pour le groupe {nameof(PermissionConstants.WifiP2pPermissionGroup)}", ToastLength.Short).Show();
                    }
                    else
                    {
                        WifiP2pManager.CreateGroup(WifiP2pChannel, new WifiP2pActionListener(this, EWifiP2pAction.CreateGroup, () =>
                        {
                            Toast.MakeText(this, "Groupe créé avec succès !", ToastLength.Short).Show();
                        }));
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        override public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo)
        {
            base.OnWifiP2pConnectionChanged(networkInfo, p2pInfo, groupInfo);
            if (networkInfo.IsConnected)
            {
                m_Views.SsidTextView.Text = groupInfo.NetworkName;

                foreach (WifiP2pDevice device in groupInfo.ClientList)
                {
                    m_Clients.Add(device);
                }

                m_Adapter.NotifyDataSetChanged();
            }
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        override public void OnWifiP2pStateChanged(EWifiState newState)
        {
            // Do nothing
        }

        private void InitializeButtons()
        {
            m_Views.DetailsButton.Click += (e, args) =>
            {
                m_Views.DetailsLayout.Visibility = m_AreDetailsShown ? ViewStates.Gone : Android.Views.ViewStates.Visible;
                m_Views.DetailsButton.Text = m_AreDetailsShown ? "Détails" : "Cacher les détails";
                m_AreDetailsShown = !m_AreDetailsShown;
            };

            m_Views.DisconnectButton.Click += (e, args) =>
            {
                WifiP2pManager.RemoveGroup(WifiP2pChannel, new WifiP2pActionListener(this, EWifiP2pAction.EndGroup));
                OnBackPressed();
            };
        }

        /// <summary>
        /// <see cref="WifiP2pActivity.OnGroupInfoAvailable(WifiP2pGroup)"/>
        /// </summary>
        override public void OnGroupInfoAvailable(WifiP2pGroup group)
        {
            foreach (WifiP2pDevice device in group.ClientList)
            {
                m_Clients.Add(device);
            }

            m_Adapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// <see cref="WifiP2pActivity.OnMessageReceivedProgressChanged(EMessageType, float)"/>
        /// </summary>
        override public void OnMessageReceivedProgressChanged(EMessageType message, float progress)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <see cref="WifiP2pActivity.OnMessageReceived(WifiP2p.Message)"/>
        /// </summary>
        override public void OnMessageReceived(WifiP2p.Message message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <see cref="WifiP2pActivity.OnMessageSendingProgressChanged(WifiP2p.Message, float)"/>
        /// </summary>
        override public void OnMessageSendingProgressChanged(WifiP2p.Message message, float progress)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <see cref="WifiP2pActivity.OnMessageSent(WifiP2p.Message)"/>
        /// </summary>
        override public void OnMessageSent(WifiP2p.Message message)
        {
            throw new System.NotImplementedException();
        }

        #endregion Methods
    }
}