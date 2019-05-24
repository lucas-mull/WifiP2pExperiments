using Android.Content;
using Android.OS;
using System;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    public class WifiP2pMessageIntent : Intent
    {
        #region Constants

        /// <summary>
        /// Action meaning data transfer (receiving) is in progress. Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionMessageReceivedProgress = "RECEIVED_DATA_PROGRESS";

        /// <summary>
        /// Action meaning data is being transferred (sent). Fetch the current progress (out of a 100) in intent extras.
        /// </summary>
        public const string ActionMessageSentProgress = "SEND_DATA_PROGRESS";

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
        public const string ExtraMessage = "EXTRA_MESSAGE";

        /// <summary>
        /// The extra param holding the extra message object.
        /// </summary>
        public const string ExtraMessageObject = "EXTRA_MESSAGE_OBJECT";

        #endregion Constants

        #region Properties

        /// <summary>
        /// The progress state between 0 and 1.
        /// </summary>
        /// <remarks>Progress being set to 1 does not necessarly mean that the operation is completed. Check <see cref="IsCompleted"/> for that.</remarks>
        public float Progress { get; private set; }

        /// <summary>
        /// Whether or not the operation is completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// The message received / sent.
        /// </summary>
        public Message Message { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialize one of the custom intent with the specified action as well as both extra parameters.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="isCompleted">Whether or not the operation is completed. Only if this is <c>true</c> can the object be retrieved safely from the message.</param>
        /// <param name="message">The message object.</param>
        public WifiP2pMessageIntent(string action, float progress, bool isCompleted, Message message) : base(action)
        {
            if (progress < 0 || progress > 1)
            {
                throw new ArgumentException($"The progress must be a value between 0 and 1 - {progress} is not a valid value.", nameof(progress));
            }

            Progress = progress;
            IsCompleted = isCompleted;
            Message = message ?? throw new ArgumentException(nameof(message));

            // Load intent with the data.
            PutAll();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Put all the data in the intent.
        /// </summary>
        private void PutAll()
        {
            this.PutExtra(ExtraProgress, Progress);
            this.PutExtra(ExtraIsOperationCompleted, IsCompleted);

            Bundle bundle = new Bundle();
            bundle.PutParcelable(ExtraMessageObject, Message);
            this.PutExtra(ExtraMessage, bundle);
        }

        /// <summary>
        /// Load the data from the intent into the properties.
        /// </summary>
        public void Load()
        {
            Progress = this.GetFloatExtra(ExtraProgress, 0);
            IsCompleted = this.GetBooleanExtra(ExtraIsOperationCompleted, false);

            Bundle bundle = this.GetBundleExtra(ExtraMessage);
            Message = bundle.GetParcelable(ExtraMessageObject) as Message;
        }

        #endregion Methods
    }
}