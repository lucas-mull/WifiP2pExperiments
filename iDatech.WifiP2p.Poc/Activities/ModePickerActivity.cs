
using Android.App;
using Android.Net.Wifi;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using iDatech.WifiP2p.Poc.Activities.Views;
using System;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class ModePickerActivity : AppCompatActivity
    {
        #region Instance variables

        /// <summary>
        /// Useful views in this layout.
        /// </summary>
        private ModePickerViews m_Views;

        /// <summary>
        /// The WiFi manager handling wifi activation state.
        /// </summary>
        private WifiManager m_WifiManager;

        #endregion Instance variables

        #region Methods

        #region Activity callbacks

        /// <summary>
        /// <see cref="Activity.OnCreate(Bundle)"/>
        /// </summary>
        override protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_mode_picker);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            m_WifiManager = (WifiManager)GetSystemService(WifiService);

            m_Views = new ModePickerViews(this);

            m_Views.WifiOffLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Gone : ViewStates.Visible;
            m_Views.WifiOnLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Visible : ViewStates.Gone;
        }

        /// <summary>
        /// <see cref="Activity.OnResume()"/>
        /// </summary>
        override protected void OnResume()
        {
            base.OnResume();

            m_Views.WifiDisabledButton.Click += ActivateWifi;
            m_Views.CreateGroupButton.Click += GoToAccessPointActivity;
            m_Views.JoinGroupButton.Click += GoToClientActivity;
        }

        /// <summary>
        /// <see cref="Activity.OnPause()"/>
        /// </summary>
        override protected void OnPause()
        {
            base.OnPause();

            m_Views.WifiDisabledButton.Click -= ActivateWifi;
            m_Views.CreateGroupButton.Click -= GoToAccessPointActivity;
            m_Views.JoinGroupButton.Click -= GoToClientActivity;
        }

        #endregion Activity callbacks

        /// <summary>
        /// Called when the "wifi disabled" button is pressed.
        /// Activates the wifi on the device.
        /// </summary>
        private void ActivateWifi(object o, EventArgs args)
        {
            m_WifiManager.SetWifiEnabled(true);
            m_Views.WifiOffLayout.Visibility = ViewStates.Gone;
            m_Views.WifiOnLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Start the <see cref="AccessPointActivity"/>
        /// </summary>
        private void GoToAccessPointActivity(object o, EventArgs args)
        {
            StartActivity(typeof(AccessPointActivity));
        }

        /// <summary>
        /// Start the <see cref="ClientActivity"/>
        /// </summary>
        private void GoToClientActivity(object e, EventArgs args)
        {
            StartActivity(typeof(ClientActivity));
        }

        #endregion Methods
    }
}