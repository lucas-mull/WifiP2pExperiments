using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace iDatech.WifiP2p.Poc.Activities.Views
{
    public class ModePickerViews
    {
        #region Properties

        /// <summary>
        /// The button displayed at the center of the screen when the wifi is disabled.
        /// </summary>
        public ImageButton WifiDisabledButton { get; private set; }

        /// <summary>
        /// The layout displayed if the wifi is OFF on startup.
        /// </summary>
        public RelativeLayout WifiOffLayout { get; private set; }

        /// <summary>
        /// The layout displayed if the wifi is ON on startup.
        /// </summary>
        public RelativeLayout WifiOnLayout { get; private set; }

        /// <summary>
        /// Button used to create a Wifi P2P group from this device.
        /// </summary>
        public Button CreateGroupButton { get; private set; }

        /// <summary>
        /// Button used to join a Wifi P2P group from this device.
        /// </summary>
        public Button JoinGroupButton { get; private set; }

        #endregion Properties

        #region Constructors

        public ModePickerViews(ModePickerActivity parent)
        {
            WifiDisabledButton = parent.FindViewById<ImageButton>(Resource.Id.btn_wifi_disabled);
            WifiOffLayout = parent.FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_off);
            WifiOnLayout = parent.FindViewById<RelativeLayout>(Resource.Id.relative_layout_wifi_on);
            CreateGroupButton = parent.FindViewById<Button>(Resource.Id.btn_create_group);
            JoinGroupButton = parent.FindViewById<Button>(Resource.Id.btn_join_group);
        }

        #endregion Constructors
    }
}