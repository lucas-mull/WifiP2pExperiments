using Android.OS;
using System;

namespace iDatech.WifiP2p.Poc.Parcelable
{
    sealed public class ParcelableCreator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object, new()
    {
        #region Instance variables

        /// <summary>
        /// The method used by the creator.
        /// </summary>
        readonly private Func<Parcel, T> m_CreateFunction;

        #endregion Instance variables

        #region Constructors

        /// <summary>
        /// Constructor specifying the create function.
        /// </summary>
        /// <param name="createFunction">The create function.</param>
        public ParcelableCreator(Func<Parcel, T> createFunction)
        {
            m_CreateFunction = createFunction ?? throw new ArgumentNullException(nameof(createFunction));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// <see cref="IParcelableCreator.CreateFromParcel(Parcel)"/>
        /// </summary>
        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            return m_CreateFunction(source);
        }

        /// <summary>
        /// <see cref="IParcelableCreator.NewArray(int)"/>
        /// </summary>
        public Java.Lang.Object[] NewArray(int size)
        {
            return new T[size];
        }

        #endregion Methods
    }
}