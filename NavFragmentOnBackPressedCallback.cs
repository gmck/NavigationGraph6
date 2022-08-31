using Android.Content;
using AndroidX.Activity;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using AndroidX.Preference;
using com.companyname.NavigationGraph6.Fragments;

namespace com.companyname.NavigationGraph6
{
    public class NavFragmentOnBackPressedCallback : OnBackPressedCallback
    {
        // Notes: OnBackPressedCallback was failing to work if instantiated in OnStart it would work in most instances, but fail on some Fragments OnDestroy where the callback is removed
        // onBackPressedCallback?.Remove();
        // base.OnDestroy();
        // onBackPressedCallback could be null and therefore the callback was not removed which subsequently stuffed up other fragments.
        // Moving  the instantiation from OnStart to OnResume appears to have fixed the problem.

        private readonly Fragment fragment;
        private readonly bool animateFragments;
        private NavOptions navOptions;

        public NavFragmentOnBackPressedCallback(Fragment fragment, bool enabled) : base(enabled)
        {
            this.fragment = fragment;
            // For animations only
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.fragment.Activity);
            animateFragments = sharedPreferences.GetBoolean("use_animations", false);
        }

        public override void HandleOnBackPressed()
        {

            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true) // 22/05/2021 We do need this
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();


            // Mixture of top level and non top level fragments
            if (fragment is HomeFragment homeFragment)
                homeFragment.HandleBackPressed();
            else if (fragment is GalleryFragment galleryFragment)
                galleryFragment.HandleBackPressed(navOptions);
            else if (fragment is SlideshowFragment slideshowFragment)
                slideshowFragment.HandleBackPressed(navOptions);
            else if (fragment is LeaderboardPagerFragment leaderboardPagerFragment)
                leaderboardPagerFragment.HandleBackPressed(navOptions);
            else if (fragment is RegisterFragment registerFragment)
                registerFragment.HandleBackPressed(navOptions);
        }
    } 
}

