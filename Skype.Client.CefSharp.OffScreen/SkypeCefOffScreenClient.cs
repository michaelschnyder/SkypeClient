using System;
using CefSharp;
using CefSharp.OffScreen;

namespace Skype.Client.CefSharp.OffScreen
{
    public class SkypeCefOffScreenClient : SkypeCefClient, IDisposable
    {
        static SkypeCefOffScreenClient()
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(new CefSettings(), performDependencyCheck: true, browserProcessHandler: null);
        }

        public SkypeCefOffScreenClient() : base(new ChromiumWebBrowser())
        {
        }

        public void Dispose()
        {
            Cef.Shutdown();
        }
    }
}
