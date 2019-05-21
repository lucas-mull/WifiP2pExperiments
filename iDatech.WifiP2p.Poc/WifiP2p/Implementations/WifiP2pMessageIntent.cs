using Android.Content;
using Android.OS;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    public class WifiP2pMessageIntent : Intent
    {
        #region Constants

        /// <summary>
        /// Action meaning a file download is in progress. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionReceivedFileProgress = "RECEIVED_FILE_PROGRESS";

        /// <summary>
        /// Action meaning data transfer (receiving) is in progress. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionReceivedDataProgress = "RECEIVED_DATA_PROGRESS";

        /// <summary>
        /// Action meaning a file is being set from the server. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionSendFileProgress = "SEND_FILE_PROGRESS";

        /// <summary>
        /// Action meaning data is being transferred (sent). Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionSendDataProgress = "SEND_DATA_PROGRESS";

        /// <summary>
        /// The extra param to fetch in the intent to get the progress (integer).
        /// </summary>
        public const string ExtraProgress = "EXTRA_PROGRESS";

        /// <summary>
        /// The extra param to fetch in the intent to check if the operation (download / upload) has completed.
        /// </summary>
        public const string ExtraIsOperationCompleted = "EXTRA_IS_OPERATION_COMPLETED";

        /// <summary>
        /// The extra param holding the extra bundle.
        /// </summary>
        public const string ExtraData = "EXTRA_DATA";

        /// <summary>
        /// The extra param holding the extra data object.
        /// </summary>
        public const string ExtraDataObject = "EXTRA_DATA_OBJECT";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initialize one of the custom intent with the specified action as well as both extra parameters.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="isCompleted">Whether or not the operation is completed.</param>
        public WifiP2pMessageIntent(string action, float progress, bool isCompleted) : base(action)
        {
            this.PutExtra(ExtraProgress, progress);
            this.PutExtra(ExtraIsOperationCompleted, isCompleted);            
        }

        #endregion Constructors
    }

    public class WifiP2pMessageIntent<TData> : WifiP2pMessageIntent where TData : class, IParcelable
    {
        #region Constructors

        public WifiP2pMessageIntent(string action, float progress, bool isCompleted, TData data = null) : base(action, progress, isCompleted)
        {
            if (data != null)
            {
                Bundle bundle = new Bundle();
                bundle.PutParcelable(ExtraDataObject, data);
                this.PutExtra(ExtraData, bundle);
            }
        }

        #endregion Constructors
    }
}