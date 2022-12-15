using Android.Graphics;
using Android.Util;
using AndroidX.Core.Content;
using AndroidX.Core.View;

namespace com.companyname.NavigationGraph6.Fragments
{
    public class ImmersiveFragment : AndroidX.Fragment.App.Fragment
    {
        private WindowInsetsControllerCompat windowInsetsControllerCompat;

        #region OnStart
        public override void OnStart()
        {
            base.OnStart();
            windowInsetsControllerCompat = WindowCompat.GetInsetsController(Activity.Window, Activity.Window.DecorView);
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();
            HideSystemUi();
        }
        #endregion

        #region OnPause        
        public override void OnPause()
        {
            base.OnPause();
            ShowSystemUi();
        }
        #endregion

        #region HideSystemUi
        public void HideSystemUi()
        {

            // 17/01/2022 Added this reference as explanation of why we needed this code from Android 11 and on...
            // Don't use android:fitsSystemWindows="true" anywhere.
            // Refer to https://stackoverflow.com/questions/57293449/go-edge-to-edge-on-android-correctly-with-windowinsets/70714398#70714398 goto the bottom for this solution

            if (Activity is MainActivity mainActivity)
            {
                mainActivity.SupportActionBar.Hide();
                mainActivity.DisableDrawerLayout();             // Disable the navigationDrawer of the MainActivity. We don't want a user to be able to swipe it into view while viewing any of the gauges
            }

            WindowCompat.SetDecorFitsSystemWindows(Activity.Window, false);
            windowInsetsControllerCompat.Hide(WindowInsetsCompat.Type.StatusBars() | WindowInsetsCompat.Type.NavigationBars());
            windowInsetsControllerCompat.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
        }
        #endregion

        #region ShowSystemUi
        private void ShowSystemUi()
        {
            //TypedValue typedValue = new TypedValue();
            //Activity.Theme.ResolveAttribute(Resource.Attribute.colorSurface, typedValue, true);
            //int color = ContextCompat.GetColor(Context, typedValue.ResourceId);
            //Activity.Window.DecorView.SetBackgroundColor(new Color(color));

            // !!!! We have a flash here when closing
            WindowCompat.SetDecorFitsSystemWindows(Activity.Window, true);
            windowInsetsControllerCompat.Show(WindowInsetsCompat.Type.StatusBars() | WindowInsetsCompat.Type.NavigationBars());

            if (Activity is MainActivity mainActivity)
            {
                mainActivity.SupportActionBar.Show();
                mainActivity.EnableDrawerLayout();
            }
        }
        #endregion
    }
}


