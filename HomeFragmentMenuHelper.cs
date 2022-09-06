using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.Lifecycle;
using Java.Interop;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.companyname.NavigationGraph6
{
    public class HomeFragmentMenuHelper : IMenuHost
    {
        private MenuHostHelper menuHostHelper = MenuHostHelper{ InvalidateMenu() }

    public HomeFragmentMenuHelper(IRunnable onInvalidateMenuCallback) : base(onInvalidateMenuCallback)
    {

    }
    //IntPtr IJavaObject.Handle => throw new NotImplementedException();

    //int IJavaPeerable.JniIdentityHashCode => throw new NotImplementedException();

    //JniObjectReference IJavaPeerable.PeerReference => throw new NotImplementedException();

    //JniPeerMembers IJavaPeerable.JniPeerMembers => throw new NotImplementedException();

    //JniManagedPeerStates IJavaPeerable.JniManagedPeerState => throw new NotImplementedException();

    void IMenuHost.AddMenuProvider(IMenuProvider p0)
    {
        throw new NotImplementedException();
    }

    void IMenuHost.AddMenuProvider(IMenuProvider p0, ILifecycleOwner p1)
    {
        throw new NotImplementedException();
    }

    void IMenuHost.AddMenuProvider(IMenuProvider p0, ILifecycleOwner p1, Lifecycle.State p2)
    {
        throw new NotImplementedException();
    }


    //void IDisposable.Dispose()
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.Disposed()
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.DisposeUnlessReferenced()
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.Finalized()
    //{
    //    throw new NotImplementedException();
    //}

    void IMenuHost.InvalidateMenu()
    {
        throw new NotImplementedException();
    }

    void IMenuHost.RemoveMenuProvider(IMenuProvider p0)
    {
        throw new NotImplementedException();
    }

    //void IJavaPeerable.SetJniIdentityHashCode(int value)
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.SetJniManagedPeerState(JniManagedPeerStates value)
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.SetPeerReference(JniObjectReference reference)
    //{
    //    throw new NotImplementedException();
    //}

    //void IJavaPeerable.UnregisterFromRuntime()
    //{
    //    throw new NotImplementedException();
    //}
}
}