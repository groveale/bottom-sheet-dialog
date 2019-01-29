using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System;
using Android.Views;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using BottomSheetDialog.EventHandlers;

namespace BottomSheetDialog
{
    [Activity(Label = "BottomSheetDialog", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private TextView mStateText;
        private TextView mOffsetText;
        private BottomSheetBehavior mBottomSheetBehavior;

        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_bottom_sheet);

            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
            mOffsetText = FindViewById<TextView>(Resource.Id.offsetText);
            mStateText = FindViewById<TextView>(Resource.Id.stateText);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            var linearLayoutManager = new LinearLayoutManager(this)
            {
                AutoMeasureEnabled = true
            };

            ApplicationAdapter adapter = new ApplicationAdapter(this, listApplications(this));
            recyclerView.SetLayoutManager(linearLayoutManager);
            recyclerView.SetAdapter(adapter);

            FrameLayout parentThatHasBottomSheetBehavior = (FrameLayout)recyclerView.Parent.Parent;
            mBottomSheetBehavior = BottomSheetBehavior.From(parentThatHasBottomSheetBehavior);

            if (mBottomSheetBehavior != null)
            {
                SetStateText(this, new OnStateChangeEventArgs(mBottomSheetBehavior.State));

                var bttomCallback = new BottomSheetBehaviorOverride(SetOffsetText, SetStateText);

                mBottomSheetBehavior.SetBottomSheetCallback(bttomCallback);
            }

            View peekButton = FindViewById<View>(Resource.Id.peek_me);
            peekButton.Click += delegate {

                //Let's peek it, programmatically
                View peakView = FindViewById<View>(Resource.Id.drag_me);
                mBottomSheetBehavior.PeekHeight = peakView.Height;
                peakView.RequestLayout();
            };

            View modal = FindViewById<View>(Resource.Id.as_modal);
            modal.Click += delegate {

                BottomSheetDialogFragment bottomSheetDialogFragment = new CustomBottomSheetDialogFragment();
                bottomSheetDialogFragment.Show(SupportFragmentManager, bottomSheetDialogFragment.Tag);
            };
           
        }




        public override void OnBackPressed()
        {
            if (mBottomSheetBehavior.State != BottomSheetBehavior.StateHidden)
            {
                mBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void SetOffsetText(object sender, OnSlideEventArgs args)
        {
            mOffsetText.Text = GetString(Resource.String.offset, args.SlideOffset);
        }

        private void SetStateText(object sender, OnStateChangeEventArgs args)
        {
            mStateText.SetText(GetStateAsString(args.NewState));
        }

        public static int GetStateAsString(int newState)
        {
            switch (newState)
            {
                case BottomSheetBehavior.StateCollapsed:
                    return Resource.String.collapsed;
                case BottomSheetBehavior.StateDragging:
                    return Resource.String.dragging;
                case BottomSheetBehavior.StateExpanded:
                    return Resource.String.expanded;
                case BottomSheetBehavior.StateHidden:
                    return Resource.String.hidden;
                case BottomSheetBehavior.StateSettling:
                    return Resource.String.settling;
            }
            return Resource.String.undefined;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            MenuInflater.Inflate(Resource.Menu.menu_bottom_sheet, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle action bar item clicks here. The action bar will
            // automatically handle clicks on the Home/Up button, so long
            // as you specify a parent activity in AndroidManifest.xml.
            int id = item.ItemId;

            //noinspection SimplifiableIfStatement
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public static List<ApplicationInfo> listApplications(Context context)
        {

            List<ApplicationInfo> installedApps = new List<ApplicationInfo>();

            PackageManager pm = context.PackageManager;
            var applications = pm.GetInstalledApplications(PackageInfoFlags.MetaData);
            foreach (var appInfo in applications)
            {
                installedApps.Add(appInfo);
            }
            return installedApps;
        }

    }

    public class ApplicationAdapter : RecyclerView.Adapter
    {
        private PackageManager mPackageManager;
        private Context mContext;

        public ApplicationAdapter(Context context, List<ApplicationInfo> mApplications)
        {
            this.mApplications = mApplications;
            mContext = context;
            mPackageManager = mContext.PackageManager;
        }

        private List<ApplicationInfo> mApplications;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationInfo applicationInfo = mApplications[position];

            // Replace the contents of the view with that element
            var myHolder = holder as ApplicationViewHolder;

            myHolder.appName.Text = applicationInfo.LoadLabel(mPackageManager);
            myHolder.appIcon.SetImageDrawable(applicationInfo.LoadIcon(mPackageManager));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;

            var app = Resource.Layout.application_item;

            itemView = LayoutInflater.From(parent.Context).Inflate(app, parent, false);

            return new ApplicationViewHolder(itemView);

        }

        public override int ItemCount => mApplications.Count;
    }

    public class ApplicationViewHolder : RecyclerView.ViewHolder
    {
        public ImageView appIcon { get; set; }
        public TextView appName { get; set;}

        public ApplicationViewHolder(View itemView) : base(itemView)
        {

            appIcon = itemView.FindViewById<ImageView>(Resource.Id.app_icon);
            appName = itemView.FindViewById<TextView>(Resource.Id.app_name);
            //itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            //itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

    }

    public class BottomSheetBehaviorOverride : BottomSheetBehavior.BottomSheetCallback
    {
        private Action<object, OnSlideEventArgs> setOffsetText;
        private Action<object, OnStateChangeEventArgs> setStateText;

        public BottomSheetBehaviorOverride(Action<object, OnSlideEventArgs> setOffsetText, Action<object, OnStateChangeEventArgs> setStateText)
        {
            this.setOffsetText = setOffsetText;
            this.setStateText = setStateText;
        }

    

        public override void OnSlide(View bottomSheet, float slideOffset)
        {
            setOffsetText.Invoke(bottomSheet, new OnSlideEventArgs(slideOffset));
        }

        public override void OnStateChanged(View bottomSheet, int newState)
        {
            setStateText.Invoke(bottomSheet, new OnStateChangeEventArgs(newState));
        }
    }

}

