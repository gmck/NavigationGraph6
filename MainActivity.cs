using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using AndroidX.Preference;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;

namespace com.companyname.NavigationGraph6
{
    [Activity(Label = "@string/app_name",  MainLauncher = true)]
    
    public class MainActivity : BaseActivity, IOnApplyWindowInsetsListener,
                                NavController.IOnDestinationChangedListener,
                                NavigationBarView.IOnItemSelectedListener,
                                NavigationView.IOnNavigationItemSelectedListener 
    {

        private AppBarConfiguration appBarConfiguration;
        private NavigationView navigationView;
        private DrawerLayout drawerLayout;
        private BottomNavigationView bottomNavigationView;
        private NavController navController;
       
        // Preference variables - see OnDestinationChanged where they are checked
        private bool devicesWithNotchesAllowFullScreen;             // allow full screen for devices with notches
        private bool animateFragments;                              // animate fragments 

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

            base.OnCreate(savedInstanceState);
            
            // Only for demonstration purposes in that you can easily see the background color and the launch icon. Remove for production build.
            //System.Threading.Thread.Sleep(500);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // This rather than android:windowTranslucentStatus in styles seems to have fixed the problem with the OK button on the BasicDialogFragment
            // It also fixes the AppBarlayout so it extends full screen, when devicesWithNotchesAllowFullScreen = true; 
            //Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            // Require a toolbar
            Google.Android.Material.AppBar.MaterialToolbar toolbar = FindViewById<Google.Android.Material.AppBar.MaterialToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            ViewCompat.SetOnApplyWindowInsetsListener(toolbar, this);

            // navigationView, bottomNavigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);

            // NavHostFragment so we can get a NavController 
            NavHostFragment navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
            navController = navHostFragment.NavController;

            
            // These are the fragments that you don't wont the back button of the toolbar to display on e.g. topLevel fragments. They correspond to the items of the NavigationView.
            int[] topLevelDestinationIds = new int[] { Resource.Id.home_fragment, Resource.Id.gallery_fragment, Resource.Id.slideshow_fragment };
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();  // SetDrawerLayout replaced with SetOpenableLayout

            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);

            // Notes using both Navigation.Fragment and Navigation.UI version 2.3.5.3. Navigation.UI therefore includes Android.Material 1.4.0.4
            // These two are working, but no animation, other than when the two fragments opened from slideshowFragment close, made possible because of HandleBackPressed(NavOptions navOptions)
            // Could fix by adding animation to the graph, but that limits the app to only one type of animation. Therefore replacing with SetNavigationItemSelectedListener 
            // That solves the problem of animating the top level fragments, but opening both fragments via the BottomNavigationView still have no animation.
            // So will replace NavigationUI.SetupWithNavController(bottomNavigationView, navController) with BottomNavigationView_ItemSelected

            //NavigationUI.SetupWithNavController(navigationView, navController);
            //NavigationUI.SetupWithNavController(bottomNavigationView, navController);

            // Upgrading to Navigation.Fragment and Navigation.UI version 2.4.2. Navigation.UI includes now Android.Material 1.5.0.2 - also tested 1.6.0
            navigationView.SetNavigationItemSelectedListener(this);
            bottomNavigationView.ItemSelected += BottomNavigationView_ItemSelected;     


            // Add the DestinationChanged listener
            navController.AddOnDestinationChangedListener(this);

            #region Notes
            // Demonstrates the problem if using 2.3.5.3 versions of Navigation and 1.4.0.4 of Material respectively
            // Already using both overloads of SetupWithNavController() and there is no provision to pass a NavOptions. Since there is no animation contained in nav_graph therefore no animation when
            // opening any fragment. 
            // The only animation is in closing the fragments as each fragment has a HandleBackPressed(NavOptions navOptions)
            // Therefore to get animation we had to drop NavigationUI.SetupWithNavController(navigationView, navController) and use navigationView.SetNavigationItemSelectedListener(this) which allows creating our
            // own NavOptions. At this point each fragment can be potentially animated both opening and closing.
            // Test: comment out NavigationUI.SetupWithNavController(navigationView, navController) and uncomment navigationView.SetNavigationItemSelectedListener(this);
            // Works as per the requirement with a choice of animations, controlled via a preference. The ugly slider animation makes checking the animation of each fragment animation easier to view.

            // Upgrade Navigation packages to latest available. 2.4.2 will include material 1.5.0.2
            // Clean, Rebuild Deploy. All seems ok until you try and open either of the BottomNavigationView Fragments after it has already been opened once. Will not open again without first closing
            // the SlideshowFragment and then opening it again and either fragment will open, but only the one time.
            // Attempts to fix.
            // Comment out NavigationUI.SetupWithNavController(bottomNavigationView, navController) and uncomment bottomNavigationView.ItemSelected, which works
            // It would appear that there is a problem with Xamarin.Google.Android.Material 1.5.0.2 
            // Next step was to add Xamarin.Google.Android.Material 1.6.0 
            // No change from 1.5.0.2 behavior - work arounds work as before.
            #endregion
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            #region Notes
            // Using a Pixel3a, which doesn't have a notch. Took some time to figure this out. The debugger was not breaking here when first starting the app or even on screen rotation even when simulating a display 
            // cutout. However once you add <item name="android:windowLayoutInDisplayCutoutMode">shortEdges</item> to your theme, which changes the default setting of LayoutInDisplayCutoutMode.Default (letter box 
            // both portrait and landscape - a terrible look). The debugger now will break at the following line. 
            #endregion

            if (v is Google.Android.Material.AppBar.MaterialToolbar)
            {
                AndroidX.Core.Graphics.Insets statusBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());

                SetMargins(v, statusBarsInsets);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    if (insets.DisplayCutout != null)
                    {
                        if (devicesWithNotchesAllowFullScreen)
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
                        else
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
                    }
                }
            }
            return insets;
        }
        #endregion

        #region SetMargins
        private void SetMargins(View v, AndroidX.Core.Graphics.Insets insets)
        {
            ViewGroup.MarginLayoutParams marginLayoutParams = (ViewGroup.MarginLayoutParams)v.LayoutParameters;
            marginLayoutParams.LeftMargin = insets.Left;
            marginLayoutParams.TopMargin = insets.Top;          // top is all we are concerned with
            marginLayoutParams.RightMargin = insets.Right;
            marginLayoutParams.BottomMargin = insets.Bottom;
            v.LayoutParameters = marginLayoutParams;
            v.RequestLayout();
        }
        #endregion

        #region OnSupportNavigationUp
        public override bool OnSupportNavigateUp()
        {
            return NavigationUI.NavigateUp(navController, appBarConfiguration) || base.OnSupportNavigateUp();
        }
        #endregion

        #region OnNavigationItemSelected
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            // Using Fader2 as the default as animateFragment is false by default - check AnimationResource.cs for different animations
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();

            bool proceed = false;

            switch (menuItem.ItemId)
            {
                // These are all topLevel fragments
                // Add fragment classes and fragment layouts as we add to the codebase as per the NavigationView items. 
                // If any classes and layouts are missing, then the NavigationView will not update the item selected.
                // The menuitem highlight will stay on the current item and the current fragment will remain displayed, nor will the app crash.
                case Resource.Id.home_fragment:
                case Resource.Id.gallery_fragment:
                case Resource.Id.slideshow_fragment:
                    proceed = true;
                    break;

                default:
                    break;
            }
            // We have the option here of animating our toplevel destinations. If we don't want animation comment out the NavOptions. 
            bool handled = false;
            if (proceed)
            {
                navController.Navigate(menuItem.ItemId, null, navOptions);
                handled = true;
            }

            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
                drawerLayout.CloseDrawer(GravityCompat.Start);

            return handled;

        }
        #endregion

        #region BottomNavigationViewItemSelected
        private void BottomNavigationView_ItemSelected(object sender, NavigationBarView.ItemSelectedEventArgs e)
        {
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();

            bool proceed = false;

            switch (e.Item.ItemId)
            {
                //case Resource.Id.holding_fragment:
                case Resource.Id.leaderboardpager_fragment:
                case Resource.Id.register_fragment:
                case Resource.Id.race_result_fragment:
                    proceed = true;
                    break;
                
                default:
                    break;
            }
            if (proceed)
                navController.Navigate(e.Item.ItemId, null, navOptions);

        }
        #endregion

        #region OnDestinationChanged
        public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle bundle)
        {

            CheckForPreferenceChanges();

            // The first menu item is not checked by default, so we need to check it to show it is selected on the startDestination fragment.
            navigationView.Menu.FindItem(Resource.Id.home_fragment).SetChecked(navDestination.Id == Resource.Id.home_fragment);

            if (navDestination.Id == Resource.Id.slideshow_fragment)
            {
                bottomNavigationView.Visibility = ViewStates.Visible;
                navigationView.Visibility = ViewStates.Gone;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            else
            {
                bottomNavigationView.Visibility = ViewStates.Gone;
                navigationView.Visibility = ViewStates.Visible;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            }

            // By default because the LeaderboardPagerFragment and the RegisterFragment are not top level fragments, they will default to showing a up button (left arrow) plus the title.
            // If you don't want the up button, remove it here. This also means that the additional code in OnSupportNavigationUp can be removed. 
            if (navDestination.Id == Resource.Id.leaderboardpager_fragment || navDestination.Id == Resource.Id.register_fragment || navDestination.Id == Resource.Id.race_result_fragment)
            {
                AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                toolbar.Title = navDestination.Label;
                toolbar.NavigationIcon = null;
            }

            #region Notes about Window.Attributes.LayoutInDisplayCutoutMode
            // Here is a bit of a trick. If we haven't set <item name="android:windowLayoutInDisplayCutoutMode">shortEdges</item> in our theme (for whatever reason). Then OnApplyWindowInsets
            // will never be called if our DrawerLayout and NavigationView have android:fitsSystemWindows="true". 

            // Therefore to guarantee that it does get called we set it here, because we don't want to letterbox our normal layouts, especially all our landscape views with the gauge views
            // Note if you do set it in styles then it should be in values-v28 or even values-v27. Android Studio gives you a warning if you try and set it in values. The problem setting in values-28 is that values-v28
            // requires the theme of the activity. Normally our theme is the splash theme and we swap it by calling SetTheme(Resource.Style.OBDTheme) in the first line of OnCreate in the MainActivity.

            // Note: Only when devicesWithNotchesAllowFullScreen is true and therefore LayoutInDisplayCutoutMode is ShortEdges will insets.DisplayCutout not be null.
            // Whenever LayoutInDisplayCutoutMode it is default or never insets.DisplayCutout will always be null.
            // So even if a device has a notch, if devicesWithNotchesAllOwFullScreen is false then will always get Default because DisplayCutout will be null.

            // Do we need this? We only need shortEdges if we have a notch, therefore why not wait until the test in OnApplyWindowInsets?
            // Answer: We do need it here, because if ShortEdges is not set here, then later in the test in OnApplyWindowInsets, insets.DisplayCutout will be null which will always result in Default being set,
            // so we can't avoid this.
            // It is really the same as if we had set ShortEdges in styles.xml of values-v28, (which we don't want to do because we are using OBDTheme.Splash). By presetting Window.Attributes.LayoutInDisplayCutoutMode
            // here, when the user tells us they want it allows insets.DisplayCutout to be not null by the time we do the test in OnApplyWindowInsets.
            // Note the Setting in Preferences has no effect if the device does not have a notch, so no harm is done if a user accidently sets devicesWithNotchesAllowFullscreen to true.
            // TODO: Make a note in our user Guide.
            #endregion

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                Window.Attributes.LayoutInDisplayCutoutMode = devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
        }
        #endregion

        #region CheckForPreferenceChanges
        private void CheckForPreferenceChanges()
        {
            // Check if anything has been changed in the Settings Fragment before re-reading and updating the preference variables
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            devicesWithNotchesAllowFullScreen = sharedPreferences.GetBoolean("devicesWithNotchesAllowFullScreen", true);
            animateFragments = sharedPreferences.GetBoolean("use_animations", false);
        }
        #endregion

        #region !! Methods only called by ImmersiveFragment if using
        public void DisableDrawerLayout() => drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
        public void EnableDrawerLayout() => drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
        #endregion

        // code removed from the MainActivity as compared to NavigationGraph5

        #region OnCreateMenu - Commented - Now not required in the MainActivity, menu functionality now in HomeFragment 
        //public void OnCreateMenu(IMenu menu, MenuInflater inflater)
        //{
        //    inflater.Inflate(Resource.Menu.main, menu);
        //}

        //public bool OnMenuItemSelected(IMenuItem menuItem)
        //{
        //    if (!animateFragments)
        //        AnimationResource.Fader2();
        //    else
        //        AnimationResource.Slider();

        //    NavOptions navOptions = new NavOptions.Builder()
        //            .SetLaunchSingleTop(true)
        //            .SetEnterAnim(AnimationResource.EnterAnimation)
        //            .SetExitAnim(AnimationResource.ExitAnimation)
        //            .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
        //            .SetPopExitAnim(AnimationResource.PopExitAnimation)
        //            .SetPopUpTo(Resource.Id.home_fragment, false, true)     // Inclusive false, saveState true.
        //            .SetRestoreState(true)
        //            .Build();

        //    if (menuItem.ItemId == Resource.Id.action_settings)
        //    {
        //        navController.Navigate(Resource.Id.settingsFragment, null, navOptions);
        //        return true;
        //    }
        //    else if (menuItem.ItemId == Resource.Id.action_subscription_info)
        //    {
        //        ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
        //        return true;
        //    }
        //    else
        //        // Maybe we just need this as the default if we transfer all this stuff above to the HomeFragment 
        //        return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(this, Resource.Id.nav_host)) || base.OnOptionsItemSelected(menuItem);
        //}
        #endregion

        #region OnOptionsItemSelected - Commented - Moved to HomeFragment - Method now called OnMenuItemSelected 
        //public override bool OnOptionsItemSelected(IMenuItem menuItem)
        //{
        //    if (!animateFragments)
        //        AnimationResource.Fader2();
        //    else
        //        AnimationResource.Slider();

        //    NavOptions navOptions = new NavOptions.Builder()
        //            .SetLaunchSingleTop(true)
        //            .SetEnterAnim(AnimationResource.EnterAnimation)
        //            .SetExitAnim(AnimationResource.ExitAnimation)
        //            .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
        //            .SetPopExitAnim(AnimationResource.PopExitAnimation)
        //            .SetPopUpTo(Resource.Id.home_fragment, false, true)     // Inclusive false, saveState true.
        //            .SetRestoreState(true)
        //            .Build();

        //    if (menuItem.ItemId == Resource.Id.action_settings)
        //    {
        //        navController.Navigate(Resource.Id.settingsFragment, null, navOptions);
        //        return true;
        //    }
        //    else if (menuItem.ItemId == Resource.Id.action_subscription_info)
        //    {
        //        ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
        //        return true;
        //    }
        //    else
        //        // Maybe we just need this as the default if we transfer all this stuff above to the HomeFragment 
        //        return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(this, Resource.Id.nav_host)) || base.OnOptionsItemSelected(menuItem);
        //}
        #endregion

        #region OnCreateOptionsMenu - Commented  - Moved to HomeFragment - Method now called OnCreateMenu
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    base.OnCreateOptionsMenu(menu);
        //    MenuInflater.Inflate(Resource.Menu.main, menu);
        //    return true;
        //}
        #endregion

        #region ShowSubscriptionInfoDialog - Commented - Moved to HomeFragment
        //private void ShowSubscriptionInfoDialog(string title, string explanation)
        //{
        //    string tag = "SubscriptionInfoDialogFragment";
        //    AndroidX.Fragment.App.FragmentManager fm = SupportFragmentManager;
        //    if (fm != null && !fm.IsDestroyed)
        //    {
        //        AndroidX.Fragment.App.Fragment fragment = fm.FindFragmentByTag(tag);
        //        if (fragment == null)
        //            BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
        //    }
        //}
        #endregion
    }
}

