using System;
using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Skype.Client.CefSharp.OffScreen
{
    public class SkypeCefOffScreenClient : SkypeCefClient, IDisposable
    {
        static SkypeCefOffScreenClient()
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(new CefSettings() { LogSeverity = LogSeverity.Error }, performDependencyCheck: true, browserProcessHandler: null);
        }

        public SkypeCefOffScreenClient() : this(NullLoggerFactory.Instance)
        {
        }

        public SkypeCefOffScreenClient(ILoggerFactory loggerFactory) : base(new ChromiumWebBrowser(), loggerFactory)
        {
        }

        public void Dispose()
        {
            Cef.Shutdown();
        }
    }
}
