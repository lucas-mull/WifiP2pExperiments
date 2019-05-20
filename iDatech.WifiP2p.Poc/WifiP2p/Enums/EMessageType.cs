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
}