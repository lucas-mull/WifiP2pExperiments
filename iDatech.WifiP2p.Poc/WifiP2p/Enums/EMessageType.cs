using iDatech.WifiP2p.Poc.WifiP2p.Implementations;
using System;
using System.Linq;

namespace iDatech.WifiP2p.Poc.WifiP2p.Enums
{
    /// <summary>
    /// Define all the messages a server / client can receive.
    /// </summary>
    public enum EMessageType : byte
    {
        /// <summary>
        /// Simple message to let the server acknowledge the client's presence.
        /// </summary>
        PingServer,

        /// <summary>
        /// Message used by the client to request the server for the SLE file.
        /// </summary>
        RequestFile,

        /// <summary>
        /// Message used by the client to send the server some data (that has been modified locally).
        /// Also used by the server to answer a <see cref="RequestUpdatedData"/> message.
        /// </summary>
        SendData,

        /// <summary>
        /// Message used by the client to request updated data from the server (that another client might have sent).
        /// </summary>
        RequestUpdatedData,

        /// <summary>
        /// Message used by the server to answer a <see cref="RequestFile"/> message.
        /// </summary>
        SendFile
    }

    /// <summary>
    /// Extension methods for the <see cref="EMessageType"/> enum.
    /// </summary>
    static public class EMessageTypeExtensions
    {
        #region Static methods

        /// <summary>
        /// Extension method to know if a specified message type is supposed to be carrying data.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns><c>true</c> if the message type comes with data, <c>false</c> otherwise.</returns>
        static public bool IsCarryingData(this EMessageType messageType)
        {
            return new[] { EMessageType.SendData, EMessageType.SendFile, EMessageType.PingServer }.Contains(messageType);
        }

        /// <summary>
        /// Extension method to get the intent action associated with a message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="isSending"><c>true</c> for sending intent, <c>false</c> for receiving intent.</param>
        /// <returns>The intent action for the specified <paramref name="messageType"/></returns>
        static public string GetIntentAction(this EMessageType messageType, bool isSending)
        {
            switch(messageType)
            {
                case EMessageType.PingServer:
                case EMessageType.SendData:
                case EMessageType.SendFile:
                    return isSending ? WifiP2pMessageIntent.ActionMessageSentProgress : WifiP2pMessageIntent.ActionMessageReceivedProgress;

                default:
                    throw new NotSupportedException($"The message type {messageType} is not associated with any intent action.");
            }
        }

        #endregion Static methods
    }
}