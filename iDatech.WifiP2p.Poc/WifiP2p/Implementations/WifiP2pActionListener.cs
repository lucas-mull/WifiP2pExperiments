using Android.Content;
using Android.Net.Wifi.P2p;
using Android.Runtime;
using Android.Widget;
using iDatech.WifiP2p.Poc.WifiP2p.Enums;
using System;

namespace iDatech.WifiP2p.Poc.WifiP2p.Implementations
{
    /// <summary>
    /// Global action listener for Wifi P2p related actions.
    /// </summary>
    sealed public class WifiP2pActionListener : Java.Lang.Object, WifiP2pManager.IActionListener
    {
        #region Instance variables

        /// <summary>
        /// The calling context.
        /// </summary>
        readonly private Context m_Context;

        /// <summary>
        /// The action this listener is listening to.
        /// </summary>
        readonly private EWifiP2pAction m_TargetAction;

        /// <summary>
        /// The callback method if the action was successful.
        /// </summary>
        readonly private Action m_OnSuccessCallback;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor initializing all parameters.
        /// </summary>
        /// <param name="context">The calling context.</param>
        /// <param name="targetAction">The action we are listening to.</param>
        /// <param name="onSuccessCallback">The callback to execute on success.</param>
        public WifiP2pActionListener(Context context, EWifiP2pAction targetAction, Action onSuccessCallback = null)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
            m_TargetAction = Enum.IsDefined(typeof(EWifiP2pAction), targetAction) ? targetAction : throw new ArgumentException($"The enum value {targetAction} is not supported.");
            m_OnSuccessCallback = onSuccessCallback;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="WifiP2pManager.IActionListener.OnFailure(WifiP2pFailureReason)"/>
        /// </summary>
        public void OnFailure([GeneratedEnum] WifiP2pFailureReason reason)
        {
            Toast.MakeText(m_Context, $"{nameof(WifiP2pActionListener)} - {m_TargetAction} failed. Reason : {reason}", ToastLength.Long).Show();
        }

        /// <summary>
        /// <see cref="WifiP2pManager.IActionListener.OnSuccess"/>
        /// </summary>
        public void OnSuccess()
        {
            // Invoke callback (if any).
            m_OnSuccessCallback?.Invoke();
        }

        #endregion Methods
    }
}