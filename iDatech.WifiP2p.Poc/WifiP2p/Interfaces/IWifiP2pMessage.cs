using iDatech.WifiP2p.Poc.WifiP2p.Enums;

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
        /// The length in bytes of the object. Zero if no object is being sent.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The object included in the message.
        /// </summary>
        object Object { get; }

        #endregion Properties
    }
}