using Android.App;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Widget;
using Java.IO;
using Java.Net;
using System.Collections.Generic;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    sealed public class ServerToClientAsyncTask : AbstractWifiP2pAsyncTask
    {
        #region Constructors

        /// <summary>
        /// <see cref="AbstractWifiP2pAsyncTask(Activity,WifiP2pInfo, int, int)"/>
        /// </summary>
        public ServerToClientAsyncTask(Activity activity, WifiP2pInfo connectionInfo, int port, int requestTimeout) : base(activity, connectionInfo, port, requestTimeout) { }

        #endregion Constructors

        #region Methods

        override protected void OnPreExecute()
        {
            base.OnPreExecute();
        }

        /// <summary>
        /// <see cref="AsyncTask.DoInBackground(Java.Lang.Object[])"/>
        /// </summary>
        override protected Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            ServerSocket serverSocket = new ServerSocket(m_Port);

            Socket clientSocket = serverSocket.Accept();

            int res;
            List<byte> buffer = new List<byte>();
            while ((res = clientSocket.InputStream.ReadByte()) != -1)
            {
                buffer.Add((byte)res);
            }

            m_Activity.RunOnUiThread(() =>
            {
                Toast.MakeText(m_Activity, $"Message received : {System.Text.Encoding.UTF8.GetString(buffer.ToArray())}", ToastLength.Short).Show();
            });

            DataOutputStream outputStream = new DataOutputStream(clientSocket.OutputStream);
            outputStream.WriteUTF("Merci !");
            outputStream.Close();

            m_Activity.RunOnUiThread(() =>
            {
                Toast.MakeText(m_Activity, $"Message sent : Merci !", ToastLength.Short).Show();
            });

            serverSocket.Close();

            return null;
        }

        #endregion Methods
    }
}