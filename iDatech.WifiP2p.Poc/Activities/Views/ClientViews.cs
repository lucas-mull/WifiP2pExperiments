using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Widget;

namespace iDatech.WifiP2p.Poc.Activities.Views
{
    public class ClientViews
    {
        #region Properties

        public ImageButton RefreshButton { get; private set; }
        public RecyclerView PeersRecyclerView { get; private set; }
        public SwipeRefreshLayout RefreshLayout { get; private set; }
        public ProgressBar GlobalProgressBar { get; private set; }
        public ProgressBar ButtonProgressBar { get; private set; }
        public RelativeLayout TitleLayout { get; private set; }
        public RelativeLayout NoPeersLayout { get; private set; }

        #endregion Properties

        #region Constructors

        public ClientViews(ClientActivity parent)
        {
            RefreshButton = parent.FindViewById<ImageButton>(Resource.Id.button_refresh);
            PeersRecyclerView = parent.FindViewById<RecyclerView>(Resource.Id.recyclerview_access_points);
            RefreshLayout = parent.FindViewById<SwipeRefreshLayout>(Resource.Id.refresh_layout);
            GlobalProgressBar = parent.FindViewById<ProgressBar>(Resource.Id.progressbar_global);
            ButtonProgressBar = parent.FindViewById<ProgressBar>(Resource.Id.progressbar_button);
            TitleLayout = parent.FindViewById<RelativeLayout>(Resource.Id.layout_title);
            NoPeersLayout = parent.FindViewById<RelativeLayout>(Resource.Id.layout_no_peers);

            PeersRecyclerView.SetLayoutManager(new LinearLayoutManager(parent));
        }

        #endregion Constructors
    }
}