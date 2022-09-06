using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationGraph6.Fragments
{
    public class SampleFragment1 : Fragment
    {
        public SampleFragment1() { }

        #region NewInstance
        internal static SampleFragment1 NewInstance()
        {
            SampleFragment1 fragment = new SampleFragment1();
            return fragment;
        }
        #endregion

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_sample1, container, false);
        }
        #endregion
    }
}