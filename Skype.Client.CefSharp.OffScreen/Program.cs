using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using Console = System.Console;

namespace Skype.Client.CefSharp.OffScreen
{
    public class Program
    {
        private static ChromiumWebBrowser _browser;

        public static int Main(string[] args)
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(new CefSettings(), performDependencyCheck: true, browserProcessHandler: null);

            // Create the offscreen Chromium browser.
            _browser = new ChromiumWebBrowser();

            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            _browser.LoadingStateChanged += BrowserLoadingStateChanged;

            Console.ReadKey();

            if (args.Length == 2)
            {
                var client = new SkypeCefClient(_browser);
                client.Login(args[0], args[1]);
            }

            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();


            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

            return 0;
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading && !string.IsNullOrEmpty(e.Browser.MainFrame.Url))
            {
                Task.Run(async () =>
                {
                    var content = await e.Browser.MainFrame.GetSourceAsync();
                    Console.WriteLine(content);
                });
            }
        }
    }
}
