
using Android.App;
using Android.Net.Wifi;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;

namespace iDatech.WifiP2p.Poc.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class ModePickerActivity : AppCompatActivity
    {
        #region Instance variables

        /// <summary>
        /// The button displayed at the center of the screen when the wifi is disabled.
        /// </summary>
        private ImageButton m_WifiDisabledButton;

        /// <summary>
        /// The layout displayed if the wifi is OFF on startup.
        /// </summary>
        private RelativeLayout m_WifiOffLayout;

        /// <summary>
        /// The layout displayed if the wifi is ON on startup.
        /// </summary>
        private RelativeLayout m_WifiOnLayout;

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

            m_WifiOffLayout = FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_off);
            m_WifiOnLayout = FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_on);
            m_WifiDisabledButton = FindViewById<ImageButton>(Resource.Id.btn_wifi_disabled);

            m_WifiOffLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Gone : ViewStates.Visible;
            m_WifiOnLayout.Visibility = m_WifiManager.IsWifiEnabled ? ViewStates.Visible : ViewStates.Gone;

            
        }

        /// <summary>
        /// <see cref="Activity.OnResume()"/>
        /// </summary>
        override protected void OnResume()
        {
            base.OnResume();

            m_WifiDisabledButton.Click += ActivateWifi;
        }

        /// <summary>
        /// <see cref="Activity.OnPause()"/>
        /// </summary>
        override protected void OnPause()
        {
            base.OnPause();

            m_WifiDisabledButton.Click -= ActivateWifi;
        }

        #endregion Activity callbacks

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

        #endregion Methods
    }
}