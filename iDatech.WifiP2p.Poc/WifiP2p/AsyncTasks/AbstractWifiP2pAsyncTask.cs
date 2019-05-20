using Android.App;
using Android.Net.Wifi.P2p;
using Android.OS;
using Java.Lang;
using System;
using System.IO;
using System.Net.Sockets;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    /// <summary>
    /// Base class for all wifi P2P related async tasks.
    /// </summary>
    abstract public class AbstractWifiP2pAsyncTask : AsyncTask
    {
        #region Instance variables

        /// <summary>
        /// The connected client socket used for transmission.
        /// </summary>
        private Socket m_Socket;

        /// <summary>
        /// The current offset for the input stream.
        /// </summary>
        private int m_CurrentReceiveOffset;

        /// <summary>
        /// The current offset for the output stream.
        /// </summary>
        private int m_CurrentSendOffset;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor using a socket
        /// </summary>
        /// <param name="socket">The connected client socket.</param>
        protected AbstractWifiP2pAsyncTask(Socket socket)
        {
            if (!socket.Connected)
            {
                throw new InvalidOperationException("The specified socket is not connected and thus no communication can occur");
            }

            m_Socket = socket;
        }

        override protected Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            // First fetch the required action. The client always asks what it wants to do first.
        }

        /// <summary>
        /// Send a generic type to the other end of the socket.
        /// </summary>
        /// <typeparam name="T">The type to transmit.</typeparam>
        /// <param name="obj">The object to send.</param>
        protected void Send<T>(T obj)
        {
            // First write the type so that the other end knows what to deserialize.
            
        }

        /// <summary>
        /// Write a generic type to the stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void Write<T>()
        {

        }

        /// <summary>
        /// Receive 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void Receive<T>()
        {
            byte[] buffer = new byte[32];
            m_Socket.
        }

        /// <summary>
        /// Send a file to the other end of the socket.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        protected void SendFile(Stream fileStream)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }
        }

        #endregion Constructors
    }
}