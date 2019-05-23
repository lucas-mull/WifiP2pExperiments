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

namespace iDatech.WifiP2p.Poc.WifiP2p.Services
{
    [Service]
    sealed public class ClientConnectingService : IntentService
    {
        #region Methods

        protected override void OnHandleIntent(Intent intent)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}