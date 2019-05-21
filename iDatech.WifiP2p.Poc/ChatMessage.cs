using Android.OS;
using Android.Runtime;
using Java.Interop;
using System;

namespace iDatech.WifiP2p.Poc
{
    sealed public class ChatMessage : Java.Lang.Object, IParcelable
    {
        #region Static variables

        /// <summary>
        /// The parcelable creator.
        /// </summary>
        static private readonly GenericParcelableCreator<ChatMessage> s_Creator = new GenericParcelableCreator<ChatMessage>(FromParcel);

        #endregion Static variables

        #region Static methods

        /// <summary>
        /// Method used by the creator to create a message from a parcel.
        /// </summary>
        /// <param name="in">The parcel.</param>
        /// <returns>The message created from the parcel.</returns>
        static public ChatMessage FromParcel(Parcel @in)
        {
            if (@in == null)
            {
                throw new ArgumentNullException(nameof(@in));
            }

            return new ChatMessage { Message = @in.ReadString() };
        }

        [ExportField("CREATOR")]
        static public GenericParcelableCreator<ChatMessage> GetCreator() => s_Creator;

        #endregion Static methods

        #region Properties

        /// <summary>
        /// The chat message.
        /// </summary>
        public string Message { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// <see cref="IParcelable.DescribeContents"/>
        /// </summary>
        public int DescribeContents()
        {
            return GetHashCode();
        }

        /// <summary>
        /// <see cref="IParcelable.WriteToParcel(Parcel, ParcelableWriteFlags)"/>
        /// </summary>
        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Message);
        }

        #endregion Methods
    }

    sealed public class GenericParcelableCreator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object, new()
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
        public GenericParcelableCreator(Func<Parcel, T> createFunction)
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