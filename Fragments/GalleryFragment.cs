using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Navigation;

namespace com.companyname.NavigationGraph6.Fragments
{
    public class GalleryFragment : Fragment
    {
        // This fragment doesn't require a menu
        private NavFragmentOnBackPressedCallback onBackPressedCallback;

        public GalleryFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_gallery, container, false);
            TextView textView = view.FindViewById<TextView>(Resource.Id.text_gallery);
            textView.Text = "This is gallery fragment";
            return view;
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            onBackPressedCallback = new NavFragmentOnBackPressedCallback(this, true);
            //// Android docs:  Strongly recommended to use the ViewLifecycleOwner.This ensures that the OnBackPressedCallback is only added when the LifecycleOwner is Lifecycle.State.STARTED.
            //// The activity also removes registered callbacks when their associated LifecycleOwner is destroyed, which prevents memory leaks and makes it suitable for use in fragments or other lifecycle owners
            //// that have a shorter lifetime than the activity.
            //// Note: this rule out using OnAttach(Context context) as the view hasn't been created yet.
            RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed(NavOptions navOptions)
        {
            onBackPressedCallback.Enabled = false;

            NavController navController = Navigation.FindNavController(Activity, Resource.Id.nav_host);

            // Navigate back to the HomeFragment
            navController.PopBackStack(Resource.Id.home_fragment, false);
            navController.Navigate(Resource.Id.home_fragment, null, navOptions);

        }
        #endregion

    }
}