using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;

namespace iDatech.WifiP2p.Poc.Parcelable
{
    /// <summary>
    /// Simple parcelable object holding information about a received file.
    /// </summary>
    sealed public class ReceivedFileInfo : Java.Lang.Object, IParcelable
    {
        #region Static variables

        /// <summary>
        /// The parcelable creator.
        /// </summary>
        static private readonly ParcelableCreator<ReceivedFileInfo> s_Creator = new ParcelableCreator<ReceivedFileInfo>(FromParcel);

        #endregion Static variables

        #region Static methods

        /// <summary>
        /// Method used by the creator to create a message from a parcel.
        /// </summary>
        /// <param name="in">The parcel.</param>
        /// <returns>The message created from the parcel.</returns>
        static public ReceivedFileInfo FromParcel(Parcel @in)
        {
            if (@in == null)
            {
                throw new ArgumentNullException(nameof(@in));
            }

            ReceivedFileInfo info = new ReceivedFileInfo
            {
                FilePath = @in.ReadString(),
                Length = @in.ReadLong()
            };

            info.Content = new byte[info.Length];
            @in.ReadByteArray(info.Content);

            return info;
        }

        /// <summary>
        /// The method used to access the creator for this parcelable.
        /// </summary>
        /// <returns>The parcelable creator.</returns>
        [ExportField("CREATOR")]
        static public ParcelableCreator<ReceivedFileInfo> GetCreator() => s_Creator;

        #endregion Static methods

        #region Constructors

        /// <summary>
        /// Parameterless constructor needed for the Parcelable interface.
        /// </summary>
        public ReceivedFileInfo() { }

        /// <summary>
        /// Constructor initializing every property.
        /// </summary>
        /// <param name="filePath"><see cref="FilePath"/></param>
        /// <param name="content"><see cref="Content"/></param>
        public ReceivedFileInfo(string filePath, byte[] content)
        {
            FilePath = !string.IsNullOrWhiteSpace(filePath) ? filePath : throw new ArgumentNullException(nameof(filePath));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Length = Content.Length;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The path to the file on the internal storage.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The length (in bytes) of the file's contents.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// The contents of the file.
        /// </summary>
        public byte[] Content { get; private set; }

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
            dest.WriteString(FilePath);
            dest.WriteLong(Length);
            dest.WriteByteArray(Content);
        }

        #endregion Methods
    }
}