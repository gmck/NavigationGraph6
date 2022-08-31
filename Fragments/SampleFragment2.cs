using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationGraph6.Fragments
{
    public class SampleFragment2 : Fragment
    {
        public SampleFragment2() { }

        #region NewInstance
        internal static SampleFragment2 NewInstance()
        {
            SampleFragment2 fragment = new SampleFragment2();
            return fragment;
        }
        #endregion

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_sample2, container, false);
        }
        #endregion
    }
}