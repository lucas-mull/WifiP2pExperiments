using Android.App;
using Android.Content;
using Android.Net.Wifi.P2p;
using Android.OS;
using Java.IO;
using Java.Net;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    sealed public class ClientReceiveAsyncTask : AbstractWifiP2pAsyncTask
    {
        #region Constructors

        /// <summary>
        /// <see cref="AbstractWifiP2pAsyncTask(Context,WifiP2pInfo, int, int)"/>
        /// </summary>
        public ClientReceiveAsyncTask(Activity activity, WifiP2pInfo connectionInfo, int port, int requestTimeout) : base(activity, connectionInfo, port, requestTimeout) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="AsyncTask.DoInBackground(Java.Lang.Object[])"/>
        /// </summary>
        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            ServerSocket serverSocket = new ServerSocket(m_Port);
            Socket clientSocket = serverSocket.Accept();
            DataOutputStream outputStream = new DataOutputStream(clientSocket.OutputStream);
            outputStream.WriteUTF("Coucou");
            outputStream.Flush();

            return null;
        }

        #endregion Methods
    }
}