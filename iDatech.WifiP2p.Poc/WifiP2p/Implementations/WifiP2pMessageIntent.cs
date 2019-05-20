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

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    sealed public class WifiP2pMessageIntent : Intent
    {
        #region Constants

        /// <summary>
        /// Action meaning a file download is in progress. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ReceivedFileProgressAction = "RECEIVED_FILE_PROGRESS";

        /// <summary>
        /// Action meaning data transfer (receiving) is in progress. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ReceivedDataProgressAction = "RECEIVED_DATA_PROGRESS";

        /// <summary>
        /// Action meaning a file is being set from the server. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string SendFileProgressAction = "SEND_FILE_PROGRESS";

        /// <summary>
        /// Action meaning data is being transferred (sent). Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string SendDataProgressAction = "SEND_DATA_PROGRESS";

        /// <summary>
        /// The extra param to fetch in the intent to get the progress (integer).
        /// </summary>
        public const string ExtraProgress = "EXTRA_PROGRESS";

        #endregion Constants

        #region Properties



        #endregion Properties

        #region Constructors

        public WifiP2pMessageIntent(string action) : base(action) { }

        #endregion Constructors
    }
}