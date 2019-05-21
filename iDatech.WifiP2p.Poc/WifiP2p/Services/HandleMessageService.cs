
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;

namespace iDatech.WifiP2p.Poc.WifiP2p.Services
{
    [Service(Exported = true, Permission = "com.google.android.gms.permission.BIND_NETWORK_TASK_SERVICE")]
    [IntentFilter(new[] { "com.google.android.gms.gcm.ACTION_TASK_READY" })]
    sealed public class HandleMessageService : GcmTaskService
    {
        public override int OnRunTask(TaskParams @params)
        {
            EMessageType messageType = (EMessageType)@params.Extras.GetByte(WifiP2pConstants.ExtraWifiP2pMessageType);
            switch (messageType)
            {
                case EMessageType.
            }
        }
    }
}