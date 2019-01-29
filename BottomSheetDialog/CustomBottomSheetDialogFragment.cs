using System;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BottomSheetDialog.EventHandlers;

namespace BottomSheetDialog
{
    public class CustomBottomSheetDialogFragment : BottomSheetDialogFragment
    {
        private TextView mOffsetText;
        private TextView mStateText;


        private LinearLayoutManager mLinearLayoutManager;
        private ApplicationAdapter mAdapter;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return base.OnCreateDialog(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            //base.OnViewCreated(view, savedInstanceState);
        }

        public override void SetupDialog(Dialog dialog, int style)
        {
            base.SetupDialog(dialog, style);

            View contentView = View.Inflate(Context, Resource.Layout.bottom_sheet_dialog_content_view, null);
            dialog.SetContentView(contentView);

            CoordinatorLayout.LayoutParams layoutParams =
                    (CoordinatorLayout.LayoutParams)((View)contentView.Parent).LayoutParameters;

            CoordinatorLayout.Behavior behavior = layoutParams.Behavior;

            if (behavior != null)
            {
                var bttomCallback = new BottomSheetBehaviorOverride(SetOffsetText, SetStateText);

                ((BottomSheetBehavior)behavior).SetBottomSheetCallback(bttomCallback);
            }

            mOffsetText = contentView.FindViewById<TextView>(Resource.Id.offsetText);
            mStateText = contentView.FindViewById<TextView>(Resource.Id.stateText);
        }

        private void SetOffsetText(object sender, OnSlideEventArgs args)
        {
            // may need to do something with runable

            mOffsetText.Text = GetString(Resource.String.offset, args.SlideOffset);
        }

        private void SetStateText(object sender, OnStateChangeEventArgs args)
        {
            mStateText.SetText(MainActivity.GetStateAsString(args.NewState));
            if (args.NewState == BottomSheetBehavior.StateHidden)
            {
                Dismiss();
            }
        }


        private class RunnableAnonymousInnerClassHelper : Java.Lang.Object, Java.Lang.IRunnable
        {
            private readonly Context outerInstance;
            private AnimationDrawable animDrawable;

            public RunnableAnonymousInnerClassHelper(Context outerInstance, AnimationDrawable animDrawable)
            {
                this.outerInstance = outerInstance;
                this.animDrawable = animDrawable;
                Console.WriteLine("*** Runnable created");
            }

            public void Run()
            {
                Console.WriteLine("*** Runnable starting");     // <-- does not show on Output panel
                animDrawable.Start();
            }
        }
    }



}


