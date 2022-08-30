using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Widget;
using com.companyname.NavigationGraph6.Adapters;
using Google.Android.Material.Tabs;
using System;


namespace com.companyname.NavigationGraph6.Fragments
{
    public class HoldingFragment : Fragment
    {
        private ViewPager2 holdingFragmentViewPager;
        private HoldingFragmentViewPagerStateAdapter holdingFragmentViewPagerStateAdapter;
        private TabLayout holdingFragmentTabLayout;
        private OnPageChangeCallback onPageChangeCallback;  // Have to register and unregister it. -- Not used
        private bool displayPageIndicator = true;           // just toggle this each way for the indicator - will eventually be an option in Settings

        public HoldingFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.fragment_holder_viewpager, container, false);
            holdingFragmentViewPager = view.FindViewById<ViewPager2>(Resource.Id.holder_viewpager);
            holdingFragmentTabLayout = view.FindViewById<TabLayout>(Resource.Id.tablayout1);
            return view;
        }
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // FragmentStateAdapter - which calls CreateFragment for the number of fragments - note the extra param ViewLifecycleOwner.Lifecycle 
            // Refer to https://stackoverflow.com/questions/61779776/leak-canary-detects-memory-leaks-for-tablayout-with-viewpager2/62184494#62184494 
            // This is using the third of the three FragmentStateAdapter public constructors.
            // Note - it is also an abstract class, so we can add out own params. e.g. passing the total number of fragments that the ViewPager2 will hold.
            holdingFragmentViewPagerStateAdapter = new HoldingFragmentViewPagerStateAdapter(ChildFragmentManager, ViewLifecycleOwner.Lifecycle, 3);

            // ViewPager2
            holdingFragmentViewPager.OffscreenPageLimit =  ViewPager2.OffscreenPageLimitDefault;
            holdingFragmentViewPager.Adapter = holdingFragmentViewPagerStateAdapter;
            holdingFragmentViewPager.SetPageTransformer(new ZoomOutPageTransformer());      // Needs some more controls in the xml of each fragment to see the effect - swipe slowly and you'll pick it up.

            // Create an OnPageChangeCallback and register it with the dashboardViewPager. We don't need it here - but keep as an example.
            onPageChangeCallback = new OnPageChangeCallback(Activity);
            holdingFragmentViewPager.RegisterOnPageChangeCallback(onPageChangeCallback);  // Unregister it in OnDestroy

            TabLayoutMediator tabMediator = new TabLayoutMediator(holdingFragmentTabLayout, holdingFragmentViewPager, new TabConfigurationStrategy());
            tabMediator.Attach();

            if (displayPageIndicator)
            {
                // Leave this - just another way of doing it, but the easier way is just to use the Dimension in dp which automatically takes care of Density
                // No this makes the bar smaller than standard and therefore better places the two gauges.
                ViewGroup.LayoutParams layoutParams = holdingFragmentTabLayout.LayoutParameters;
                layoutParams.Height = (int)(16 * Resources.DisplayMetrics.Density);
                holdingFragmentTabLayout.LayoutParameters = layoutParams;
                TogglePageIndicatorClickable(false);
            }
            else
                holdingFragmentTabLayout.Visibility = ViewStates.Gone;
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            holdingFragmentViewPager.UnregisterOnPageChangeCallback(onPageChangeCallback);
            base.OnDestroy();
        }
        #endregion

        #region TogglePageIndicatorClickable
        private void TogglePageIndicatorClickable(bool enable = false)
        {
            // TODO: We would need a listener if we want to turn this feature on or off.
            // Ideally normally each indicator would be clickable. When the the statusBar and NavigationBar appear make them non clickable
            foreach (View view in holdingFragmentTabLayout.Touchables)
                view.Clickable = enable;
        }
        #endregion

        #region  TabConfigurationStrategy
        private class TabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            // No need to do anything here - empty method as the tablayout itself is supplying the tab selector - see the xml.
            public void OnConfigureTab(TabLayout.Tab p0, int p1)
            {

            }
        }
        #endregion

        #region ZoomPageTransformer
        public class ZoomOutPageTransformer : Java.Lang.Object, ViewPager2.IPageTransformer
        {
            private static float MinimumScale = 0.85f;
            private static float MinimumAlpha = 0.5f;

            public void TransformPage(View view, float position)
            {
                int pageWidth = view.Width;
                int pageHeight = view.Height;

                // page filled position = 0
                // page when drawn just off to right position = 1
                // page when drawn just off to left position = -1

                if (position < -1)
                {   // [-Infinity,-1)
                    // This page is way off-screen to the left.
                    view.Alpha = 0f;
                }
                else if (position <= 1)
                {
                    // e.g. -1,1
                    // Modify the default slide transition to shrink the page as well
                    float scaleFactor = Math.Max(MinimumScale, 1 - Math.Abs(position));
                    float vertMargin = pageHeight * (1 - scaleFactor) / 2;
                    float horzMargin = pageWidth * (1 - scaleFactor) / 2;
                    if (position < 0)
                        view.TranslationX = (horzMargin - vertMargin / 2);
                    else
                        view.TranslationX = (-horzMargin + vertMargin / 2);

                    // Scale the page down (between MinimumScale and 1)
                    view.ScaleX = scaleFactor;
                    view.ScaleY = scaleFactor;

                    // Fade the page relative to its size.
                    view.Alpha = MinimumAlpha + (scaleFactor - MinimumScale) / (1 - MinimumScale) * (1 - MinimumAlpha);
                }
                else
                    // This page is way off-screen to the right.
                    view.Alpha = 0f;
            }
        }
        #endregion

        #region OnPageChangeCallback - Not using
        private class OnPageChangeCallback : ViewPager2.OnPageChangeCallback
        {
            // We don't really need this. But would for instance if we wanted to display a Toast
            private readonly Context context;

            public OnPageChangeCallback(Context context)
            {
                this.context = context;
            }

            public override void OnPageSelected(int position)
            {

            }
        }
        #endregion

    }
}