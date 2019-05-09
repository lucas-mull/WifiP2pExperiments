using Android.App;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Net;
using System.Collections.Generic;

namespace iDatech.WifiP2p.Poc.WifiP2p.AsyncTasks
{
    sealed public class PingServerAsyncTask : AbstractWifiP2pAsyncTask<Java.Lang.Object, Java.Lang.Object, string>
    {
        #region Constructors

        /// <summary>
        /// <see cref="AbstractWifiP2pAsyncTask(Activity,WifiP2pInfo, int, int)"/>
        /// </summary>
        public PingServerAsyncTask(Activity activity, WifiP2pInfo connectionInfo, int port, int requestTimeout) : base(activity, connectionInfo, port, requestTimeout) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="AsyncTask{Object, Object, string}.DoInBackground(Object[])"/>
        /// </summary>
        override protected string RunInBackground(params Object[] @params)
        {
            string hostName = m_ConnectionInfo.GroupOwnerAddress.HostAddress;
            Socket clientSocket = new Socket();

            try
            {
                clientSocket.Bind(null);
                clientSocket.Connect(new InetSocketAddress(hostName, m_Port), m_RequestTimeout);

                m_Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(m_Activity, $"Connected to : {hostName}", ToastLength.Short).Show();
                });

                //DataOutputStream os = new DataOutputStream(clientSocket.OutputStream);
                //os.WriteUTF("Coucou");
                //os.Flush();

                //byte[] buffer = new byte[clientSocket.InputStream.Length];
                //clientSocket.InputStream.Read(buffer, 0, (int)clientSocket.InputStream.Length);

                int res;
                List<byte> buffer = new List<byte>();
                while ((res = clientSocket.InputStream.ReadByte()) != -1)
                {
                    buffer.Add((byte)res);
                }

                m_Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(m_Activity, $"Message reçu : {System.Text.Encoding.UTF8.GetString(buffer.ToArray())}", ToastLength.Short).Show();
                });


                return "Coucou";
            }
            catch (System.Exception e)
            {
                // Silent exceptions for now.
            }
            finally
            {
                if (clientSocket != null && clientSocket.IsConnected)
                {
                    clientSocket.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// <see cref="AsyncTask{Object,Object,string}.OnPostExecute(string)"/>
        /// </summary>
        override protected void OnPostExecute(string result)
        {
            base.OnPostExecute(result);

            //Toast.MakeText(m_Activity, $"Message received : {result}", ToastLength.Long).Show();
        }

        #endregion Methods
    }
}