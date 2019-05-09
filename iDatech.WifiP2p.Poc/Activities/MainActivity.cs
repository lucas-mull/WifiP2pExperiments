using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Net.Wifi;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using iDatech.WifiP2p.Poc.Permissions;
using iDatech.WifiP2p.Poc.ViewGroups;
using iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using iDatech.WifiP2p.Poc.WifiP2p.Interfaces;
using System;
using System.Linq;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, WifiP2pManager.IChannelListener, IWifiP2pCallbacksHandler, WifiP2pManager.IGroupInfoListener
    {
        #region Instance variables

        /// <summary>
        /// The WiFi P2P manager handling discovering and connections using wifi direct.
        /// </summary>
        private WifiP2pManager m_WifiP2pManager;

        /// <summary>
        /// The WiFi manager handling wifi activation state.
        /// </summary>
        private WifiManager m_WifiManager;

        /// <summary>
        /// The permission service.
        /// </summary>
        private IPermissionService m_PermissionService;

        /// <summary>
        /// The main channel used to discover peers.
        /// </summary>
        private WifiP2pManager.Channel m_MainChannel;

        /// <summary>
        /// Broadcast receiver for Wifi P2p events.
        /// </summary>
        private WifiP2pBroadcastReceiver m_WifiP2pBroadcastReceiver;

        /// <summary>
        /// Button used to start discovering devices.
        /// </summary>
        private ImageButton m_DiscoverButton;

        /// <summary>
        /// The progress bar displayed while discovering devices.
        /// </summary>
        private ProgressBar m_DiscoverProgressBar;

        /// <summary>
        /// The button used to start the discovery
        /// </summary>
        private FloatingActionButton m_Fab;

        /// <summary>
        /// The button displayed at the center of the screen when the wifi is disabled.
        /// </summary>
        private ImageButton m_WifiDisabledButton;

        /// <summary>
        /// The recycler view holding the list of discovered devices.
        /// </summary>
        private RecyclerView m_WifiPeersRecyclerView;

        /// <summary>
        /// The layout displayed if the wifi is OFF on startup.
        /// </summary>
        private RelativeLayout m_WifiOffLayout;

        /// <summary>
        /// The layout displayed if the wifi is ON on startup.
        /// </summary>
        private RelativeLayout m_WifiOnLayout;

        /// <summary>
        /// View group for Wifi P2P group information display.
        /// </summary>
        private GroupInfoViewGroup m_GroupInfo;

        /// <summary>
        /// View group for Wifi P2P connection information display.
        /// </summary>
        private ConnectionInfoViewGroup m_ConnectionInfo;

        /// <summary>
        /// The intent filter used for the <see cref="WifiP2pBroadcastReceiver"/>
        /// </summary>
        private IntentFilter m_IntentFilter;

        /// <summary>
        /// Used to keep track of user input after the permissions are requested.
        /// </summary>
        private bool m_WasDiscoveryRequested;

        /// <summary>
        /// Is the device currently connected ?
        /// </summary>
        private bool m_IsConnected;

        #endregion Instance variables

        #region Methods

        #region Activity overrides

        /// <summary>
        /// <see cref="Activity.OnCreate(Bundle)"/>
        /// </summary>
        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //m_Fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //m_Fab.Click += FabOnClick;

            m_WifiP2pManager = (WifiP2pManager)GetSystemService(WifiP2pService);
            m_MainChannel = m_WifiP2pManager.Initialize(this, MainLooper, null);

            m_WifiManager = (WifiManager)GetSystemService(WifiService);

            m_WifiP2pBroadcastReceiver = new WifiP2pBroadcastReceiver(m_WifiP2pManager, m_MainChannel, this);

            m_PermissionService = new PermissionService(this);

            m_IntentFilter = new IntentFilter();
            m_IntentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
            m_IntentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
            m_IntentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
            m_IntentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);

            m_DiscoverButton = FindViewById<ImageButton>(Resource.Id.discoveryRefreshButton);
            m_DiscoverProgressBar = FindViewById<ProgressBar>(Resource.Id.discoveryProgressBar);
            m_WifiPeersRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewNewDevices);
            m_WifiDisabledButton = FindViewById<ImageButton>(Resource.Id.btn_wifi_disabled);
            m_WifiOffLayout = FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_off);
            m_WifiOnLayout = FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_on);

            m_GroupInfo = new GroupInfoViewGroup
            {
                SsidTextView = FindViewById<TextView>(Resource.Id.txt_ssid),
                PassphraseTextView = FindViewById<TextView>(Resource.Id.txt_passphrase),
                MembersRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewGroupMembers)
            };

            m_GroupInfo.MembersRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            m_ConnectionInfo = new ConnectionInfoViewGroup
            {
                StatusTextView = FindViewById<TextView>(Resource.Id.txt_status),
                TypeTextView = FindViewById<TextView>(Resource.Id.txt_type)
            };

            m_WifiPeersRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            m_DiscoverProgressBar.Visibility = ViewStates.Gone;
            m_DiscoverButton.Click += OnDiscoveryButtonPressed;
            m_WifiDisabledButton.Click += ActivateWifi;

            m_WifiOffLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Gone : ViewStates.Visible;
            m_WifiOnLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Visible : ViewStates.Gone;
        }

        /// <summary>
        /// <see cref="Activity.OnDestroy"/>
        /// </summary>
        override protected void OnDestroy()
        {
            base.OnDestroy();

            m_DiscoverButton.Click -= OnDiscoveryButtonPressed;
            m_WifiDisabledButton.Click -= ActivateWifi;
        }

        /// <summary>
        /// <see cref="Activity.OnPause"/>
        /// </summary>
        override protected void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(m_WifiP2pBroadcastReceiver);
        }

        /// <summary>
        /// <see cref="Activity.OnResume"/>
        /// </summary>
        override protected void OnResume()
        {
            base.OnResume();

            RegisterReceiver(m_WifiP2pBroadcastReceiver, m_IntentFilter);
        }

        /// <summary>
        /// <see cref="Activity.OnCreateOptionsMenu(IMenu)"/>
        /// </summary>
        override public bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        /// <summary>
        /// <see cref="Activity.OnOptionsItemSelected(IMenuItem)"/>
        /// </summary>
        override public bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
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
                        DiscoverPeers();
                    }

                    break;

                default:
                    break;
            }
        }

        #endregion Activity overrides

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Snackbar.Make(m_Fab, "Création d'un point d'accès...", Snackbar.LengthLong).Show();

            if (!m_IsConnected)
            {
                m_WifiP2pManager.CreateGroup(m_MainChannel, new WifiP2pActionListener(this, EWifiP2pAction.CreateGroup));
            }
            else
            {
                m_WifiP2pManager.RemoveGroup(m_MainChannel, new WifiP2pActionListener(this, EWifiP2pAction.EndGroup));
            }
        }

        private void OnDiscoveryButtonPressed(object o, EventArgs args)
        {
            if (!m_PermissionService.IsPermissionGroupGranted(PermissionConstants.WifiP2pPermissionGroup))
            {
                m_WasDiscoveryRequested = true;
                m_PermissionService.RequestPermissions(PermissionConstants.WifiP2pPermissionsGroupId, PermissionConstants.WifiP2pPermissionGroup);
            }

            DiscoverPeers();
        }

        /// <summary>
        /// Start discovering peers.
        /// </summary>
        private void DiscoverPeers()
        {
            m_DiscoverButton.Visibility = ViewStates.Gone;
            m_DiscoverProgressBar.Visibility = ViewStates.Visible;

            m_WifiP2pManager.DiscoverPeers(m_MainChannel, new WifiP2pActionListener(this, EWifiP2pAction.Discover));
            m_WasDiscoveryRequested = false;
        }

        public void OnChannelDisconnected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the "wifi disabled" button is pressed.
        /// Activates the wifi on the device.
        /// </summary>
        private void ActivateWifi(object o, EventArgs args)
        {
            m_WifiManager.SetWifiEnabled(true);
            m_WifiOffLayout.Visibility = ViewStates.Gone;
            m_WifiOnLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pConnectionChanged(NetworkInfo, WifiP2pInfo, WifiP2pGroup)"/>
        /// </summary>
        public void OnWifiP2pConnectionChanged(NetworkInfo networkInfo, WifiP2pInfo p2pInfo, WifiP2pGroup groupInfo)
        {
            m_IsConnected = networkInfo.IsConnected;
            if (m_IsConnected)
            {
                m_ConnectionInfo.StatusTextView.Text = "Connecté";
                if (p2pInfo.GroupFormed)
                {
                    m_ConnectionInfo.TypeTextView.Text = p2pInfo.IsGroupOwner ? "Serveur" : "Client";
                    m_GroupInfo.PassphraseTextView.Text = groupInfo.Passphrase;
                    m_GroupInfo.SsidTextView.Text = groupInfo.NetworkName;
                    m_GroupInfo.MembersRecyclerView.SetAdapter(new WifiPeerAdapter(groupInfo.ClientList.ToList(), null));
                    m_GroupInfo.MembersRecyclerView.GetAdapter().NotifyDataSetChanged();

                    if (p2pInfo.IsGroupOwner)
                    {
                        new ServerToClientAsyncTask(this, p2pInfo, 8888, 500).Execute();
                    }
                    else
                    {
                        new PingServerAsyncTask(this, p2pInfo, 8888, 500).Execute();
                    }
                }
                else
                {
                    m_ConnectionInfo.TypeTextView.Text = "Non défini";
                }
            }
            else
            {
                m_ConnectionInfo.StatusTextView.Text = "Déconnecté";
                m_ConnectionInfo.TypeTextView.Text = "Non défini";
                m_GroupInfo.PassphraseTextView.Text = "-";
                m_GroupInfo.SsidTextView.Text = "-";
                m_GroupInfo.MembersRecyclerView.SetAdapter(null);
            }
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnWifiP2pStateChanged(EWifiState)"/>
        /// </summary>
        public void OnWifiP2pStateChanged(EWifiState newState)
        {
            // Do nothing
        }

        /// <summary>
        /// <see cref="IWifiP2pCallbacksHandler.OnThisDeviceChanged(WifiP2pDevice)"/>
        /// </summary>
        public void OnThisDeviceChanged(WifiP2pDevice deviceDetails)
        {
            // Do nothing
        }

        /// <summary>
        /// <see cref="WifiP2pManager.IPeerListListener.OnPeersAvailable(WifiP2pDeviceList)"/>
        /// </summary>
        public void OnPeersAvailable(WifiP2pDeviceList peers)
        {
            m_DiscoverButton.Visibility = ViewStates.Visible;
            m_DiscoverProgressBar.Visibility = ViewStates.Gone;

            WifiPeerAdapter adapter = new WifiPeerAdapter(peers.DeviceList.ToList(), device =>
            {
                if (m_IsConnected)
                {
                    m_WifiP2pManager.RemoveGroup(m_MainChannel, new WifiP2pActionListener(this, EWifiP2pAction.Disconnect, () =>
                    {
                        Toast.MakeText(this, "Disconnection successful", ToastLength.Short).Show();
                    }));
                }
                else
                {
                    WifiP2pConfig config = new WifiP2pConfig();
                    config.DeviceAddress = device.DeviceAddress;
                    config.Wps.Setup = WpsInfo.Pbc;
                    config.GroupOwnerIntent = 1;
                    m_WifiP2pManager.Connect(m_MainChannel, config, new WifiP2pActionListener(this, EWifiP2pAction.Connect, () =>
                    {
                        Toast.MakeText(this, $"Connection to {device.DeviceName} successful", ToastLength.Short).Show();
                    }));
                }
            });

            m_WifiPeersRecyclerView.SetAdapter(adapter);
            m_WifiPeersRecyclerView.GetAdapter().NotifyDataSetChanged();
        }

        public void OnGroupInfoAvailable(WifiP2pGroup group)
        {
            m_GroupInfo.PassphraseTextView.Text = group.Passphrase;
            m_GroupInfo.SsidTextView.Text = group.NetworkName;
            m_GroupInfo.MembersRecyclerView.SetAdapter(new WifiPeerAdapter(group.ClientList.ToList(), null));
            m_GroupInfo.MembersRecyclerView.GetAdapter().NotifyDataSetChanged();
        }

        #endregion Methods
    }
}

