using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
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
        }
        #endregion
    }
}
