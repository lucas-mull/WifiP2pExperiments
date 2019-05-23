using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;

namespace iDatech.WifiP2p.Poc.Parcelable
{
    /// <summary>
    /// Parcelable class containing the device info.
    /// This is sent to the server on a first connection to establish the info for each client.
    /// </summary>
    sealed public class DeviceInfo : Java.Lang.Object, IParcelable
    {
        #region Static variables

        /// <summary>
        /// The parcelable creator.
        /// </summary>
        static private readonly ParcelableCreator<DeviceInfo> s_Creator = new ParcelableCreator<DeviceInfo>(FromParcel);

        #endregion Static variables

        #region Static methods

        /// <summary>
        /// Method used by the creator to create a message from a parcel.
        /// </summary>
        /// <param name="in">The parcel.</param>
        /// <returns>The message created from the parcel.</returns>
        static public DeviceInfo FromParcel(Parcel @in)
        {
            if (@in == null)
            {
                throw new ArgumentNullException(nameof(@in));
            }

            return new DeviceInfo
            {
                Name = @in.ReadString(),
                MacAddress = @in.ReadString(),
                IPAddress = @in.ReadString()
            };
        }

        /// <summary>
        /// The method used to access the creator for this parcelable.
        /// </summary>
        /// <returns>The parcelable creator.</returns>
        [ExportField("CREATOR")]
        static public ParcelableCreator<DeviceInfo> GetCreator() => s_Creator;

        #endregion Static methods

        #region Properties

        /// <summary>
        /// The device name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The device mac address.
        /// </summary>
        public string MacAddress { get; private set; }

        /// <summary>
        /// The device IP address.
        /// </summary>
        public string IPAddress { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor initializing all the parameters.
        /// </summary>
        /// <param name="name">The device name.</param>
        /// <param name="macAddress">The device mac address.</param>
        /// <param name="ipAddress">The device IP address.</param>
        public DeviceInfo(string name, string macAddress, string ipAddress)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            MacAddress = !string.IsNullOrWhiteSpace(macAddress) ? macAddress : throw new ArgumentNullException(nameof(macAddress));
            IPAddress = !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : throw new ArgumentNullException(nameof(ipAddress));
        }

        /// <summary>
        /// Parameterless constructor needed for the parcelable interface.
        /// </summary>
        public DeviceInfo() { }

        #endregion Constructors

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
            dest.WriteString(Name);
            dest.WriteString(MacAddress);
            dest.WriteString(IPAddress);
        }

        #endregion Methods
    }
}