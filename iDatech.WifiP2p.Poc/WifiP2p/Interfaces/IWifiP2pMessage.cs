using Android.OS;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using System.IO;

namespace iDatech.WifiP2p.Poc.WifiP2p.Interfaces
{
    /// <summary>
    /// Base interface (parameterless).
    /// </summary>
    public interface IWifiP2pMessage
    {
        #region Properties

        /// <summary>
        /// Id of the message type. Contained on one byte.
        /// </summary>
        EMessageType MessageType { get; }

        /// <summary>
        /// The length in bytes of the object / file. Zero if no object / file is being sent.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// The object included in the message.
        /// </summary>
        IParcelable Object { get; }

        #endregion Properties
    }
}