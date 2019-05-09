using Android.Views;
using System;
using static Android.Views.View;

namespace iDatech.WifiP2p.Poc
{
    /// <summary>
    /// Base class inheriting <see cref="IOnClickListener"/> and allowing for the use of a delegate.
    /// </summary>
    public class BaseOnClickListener : Java.Lang.Object, IOnClickListener
    {
        #region Instance variables

        /// <summary>
        /// On click callback.
        /// </summary>
        readonly private Action<View> m_OnClick;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="onClick">On click callback.</param>
        public BaseOnClickListener(Action<View> onClick)
        {
            m_OnClick = onClick;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="IOnClickListener.OnClick(View)"/>
        /// </summary>
        public void OnClick(View v)
        {
            m_OnClick?.Invoke(v);
        }

        #endregion Methods
    }
}