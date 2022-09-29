using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using AndroidX.Preference;

namespace com.companyname.NavigationGraph6
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected ISharedPreferences sharedPreferences;
        private string requestedColorTheme;
        
        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Sets whether the decor view should fit root-level content views for WindowInsetsCompat.
            // In other words - 
            // The single argument controls whether or not our layout will fit inside the system windows (if true), or be drawn behind them (if false). 
            //WindowCompat.SetDecorFitsSystemWindows(Window, false);

            // This rather than android:windowTranslucentStatus in styles seems to have fixed the problem with the OK button on the BasicDialogFragment
            // It also fixes the AppBarlayout so it extends full screen, when devicesWithNotchesAllowFullScreen = true; 
            // Comment this out to see the result of the AppBarLayout
            Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            // colorThemeValue defaults to RedBmw
            requestedColorTheme = sharedPreferences.GetString("colorThemeValue", "1");
            SetAppTheme(requestedColorTheme);
        }
        #endregion

        #region SetAppTheme
        private void SetAppTheme(string requestedColorTheme)
        {
            if (requestedColorTheme == "1")
                SetTheme(Resource.Style.Theme_NavigationGraph_RedBmw);
            else if (requestedColorTheme == "2")
                SetTheme(Resource.Style.Theme_NavigationGraph_BlueAudi);
            else if (requestedColorTheme == "3")
                SetTheme(Resource.Style.Theme_NavigationGraph_GreenBmw);

            TypedValue typedValue = new TypedValue();
            Theme.ResolveAttribute(Resource.Attribute.colorPrimaryVariant, typedValue, true);
            int color = ContextCompat.GetColor(this, typedValue.ResourceId);
            Window.SetStatusBarColor(new Color(color));
        }
        #endregion
    }
}
