using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;

namespace iDatech.WifiP2p.Poc.Parcelable
{
    /// <summary>
    /// Simple parcelable object used to send a chat message.
    /// </summary>
    sealed public class ChatMessage : Java.Lang.Object, IParcelable
    {
        #region Static variables

        /// <summary>
        /// The parcelable creator.
        /// </summary>
        static private readonly ParcelableCreator<ChatMessage> s_Creator = new ParcelableCreator<ChatMessage>(FromParcel);

        #endregion Static variables

        #region Static methods

        /// <summary>
        /// Method used by the creator to create a message from a parcel.
        /// </summary>
        /// <param name="in">The parcel.</param>
        /// <returns>The message created from the parcel.</returns>
        static public ChatMessage FromParcel(Parcel @in)
        {
            if (@in == null)
            {
                throw new ArgumentNullException(nameof(@in));
            }

            return new ChatMessage
            {
                Message = @in.ReadString(),
                SenderAddress = @in.ReadString()
            };
        }

        /// <summary>
        /// The method used to access the creator for this parcelable.
        /// </summary>
        /// <returns>The parcelable creator.</returns>
        [ExportField("CREATOR")]
        static public ParcelableCreator<ChatMessage> GetCreator() => s_Creator;

        #endregion Static methods

        #region Constructors

        /// <summary>
        /// Parameterless constructor needed for the Parcelable interface.
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Default constructor initializing every property.
        /// </summary>
        /// <param name="message">The message.</param>
        public ChatMessage(string message)
        {
            Message = message;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The chat message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The mac address of the device sending the message.
        /// </summary>
        public string SenderAddress { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// <see cref="IParcelable.DescribeContents"/>
        /// </summary>
        public int DescribeContents()
        {
            return GetHashCode();
        }

        /// <summary>
        /// <see cref="IParcelable.WriteToParcel(Parcel, ParcelableWriteFlags)"/>
        /// </summary>
        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Message);
            dest.WriteString(SenderAddress);
        }

        #endregion Methods
    }
}