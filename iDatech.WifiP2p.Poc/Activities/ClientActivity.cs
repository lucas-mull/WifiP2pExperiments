
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
using iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using System.Collections.Generic;
using System.Linq;
using static Android.Support.V4.Widget.SwipeRefreshLayout;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "ClientActivity")]
    public class ClientActivity : AbstractWifiP2pActivity, IOnRefreshListener
    {
        #region Constants

        /// <summary>
        /// The discover time out (in seconds).
        /// </summary>
        private const float DiscoverTimeOut = 5f;

        #endregion Constants

        #region Instance variables

        /// <summary>
        /// The useful views in this layout.
        /// </summary>
        private ClientViews m_Views;

        /// <summary>
        /// The list of peers that have been found since the last discovery.
        /// </summary>
        /// <remarks>Reset each time a new discovery is started.</remarks>
        private HashSet<WifiP2pDevice> m_CurrentPeers;

        /// <summary>
        /// The adapter for the recycler view.
        /// </summary>
        private WifiPeerAdapter m_Adapter;

        /// <summary>
        /// The permission service.
        /// </summary>
        private IPermissionService m_PermissionService;

        /// <summary>
        /// Keep track of the discovery request in case of insuffisant permissions.
        /// </summary>
        private bool m_WasDiscoveryRequested = false;

        #endregion Instance variables

        #region Methods       

        /// <summary>
        /// <see cref="Activity.OnCreate(Bundle)"/>
        /// </summary>
        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_client);

            m_Views = new ClientViews(this);

            m_Views.ButtonProgressBar.Visibility = ViewStates.Gone;
            m_Views.GlobalProgressBar.Visibility = ViewStates.Visible;
            m_Views.TitleLayout.Visibility = ViewStates.Gone;
            m_Views.NoPeersLayout.Visibility = ViewStates.Gone;
            m_Views.RefreshLayout.SetOnRefreshListener(this);
            m_Views.RefreshButton.SetOnClickListener(new BaseOnClickListener(v =>
            {
                m_Views.RefreshButton.Visibility = ViewStates.Invisible;
                m_Views.ButtonProgressBar.Visibility = ViewStates.Visible;

                StartDiscovering();
            }));

            m_CurrentPeers = new HashSet<WifiP2pDevice>();
            m_Adapter = new WifiPeerAdapter(m_CurrentPeers, false, device =>
            {
                WifiP2pConfig config = new WifiP2pConfig
                {
                    DeviceAddress = device.DeviceAddress
                };

                WifiP2pManager.Connect(WifiP2pChannel, config, new WifiP2pActionListener(this, EWifiP2pAction.Connect));
            });

            m_Views.PeersRecyclerView.SetAdapter(m_Adapter);

            m_PermissionService = new PermissionService(this);

            StartDiscovering();
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
                    else if (m_WasDiscoveryRequested)
                    {
                        StartDiscovering();
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// <see cref="AbstractWifiP2pActivity.OnPeersAvailable(WifiP2pDeviceList)"/>
        /// </summary>
        override public void OnPeersAvailable(WifiP2pDeviceList peers)
        {
            m_Views.GlobalProgressBar.Visibility = ViewStates.Gone;
            m_Views.TitleLayout.Visibility = ViewStates.Visible;
            m_WasDiscoveryRequested = false;

            foreach(WifiP2pDevice device in peers.DeviceList)
            {
                m_CurrentPeers.Add(device);
            }

            m_Adapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// <see cref="AbstractWifiP2pActivity.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        override public void OnThisDeviceChanged(WifiP2pDevice deviceDetails)
        {
            // Do nothing
        }

        /// <summary>
        /// <see cref="AbstractWifiP2pActivity.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        override public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo)
        {
            // TODO
        }

        /// <summary>
        /// <see cref="AbstractWifiP2pActivity.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        override public void OnWifiP2pStateChanged(EWifiState newState)
        {
            // TODO Wifi P2P enabled / disabled.
        }

        /// <summary>
        /// Start discovering peers.
        /// </summary>
        private void StartDiscovering()
        {
            if (!m_PermissionService.IsPermissionGroupGranted(PermissionConstants.WifiP2pPermissionGroup))
            {
                m_WasDiscoveryRequested = true;
                m_PermissionService.RequestPermissions(PermissionConstants.WifiP2pPermissionsGroupId, PermissionConstants.WifiP2pPermissionGroup);
                return;
            }

            DiscoverAsyncTask discoveryTask = new DiscoverAsyncTask(this, WifiP2pManager, WifiP2pChannel, DiscoverTimeOut);
            discoveryTask.DiscoveryStopped += OnDiscoveryStopped;
            discoveryTask.Execute();

            m_CurrentPeers.Clear();
        }

        /// <summary>
        /// Called when the peer discovery has stopped.
        /// </summary>
        private void OnDiscoveryStopped()
        {
            m_Views.GlobalProgressBar.Visibility = ViewStates.Gone;
            m_Views.ButtonProgressBar.Visibility = ViewStates.Gone;
            m_Views.RefreshButton.Visibility = ViewStates.Visible;
            m_Views.RefreshLayout.Refreshing = false;
            m_WasDiscoveryRequested = false;

            if (!m_CurrentPeers.Any())
            {
                m_Views.TitleLayout.Visibility = ViewStates.Gone;
                m_Views.NoPeersLayout.Visibility = ViewStates.Visible;
            }
        }

        /// <summary>
        /// <see cref="IOnRefreshListener.OnRefresh"/>
        /// </summary>
        public void OnRefresh()
        {
            m_Views.NoPeersLayout.Visibility = ViewStates.Gone;
            StartDiscovering();
        }

        #endregion Methods
    }
}