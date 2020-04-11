using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Extensions;
using CefSharp.Extensions.Interception;
using CefSharp.Internals;
using SkypeWebPageHost.Container;

namespace SkypeWebPageHost
{
    public class SkypeApp
    {
        private static string SkypeWebAppUrl = "https://web.skype.com/";

        private readonly IRenderWebBrowser _browser;
        private readonly WebElement _webElement;

        public SkypeApp(IRenderWebBrowser browser)
        {
            _browser = browser;
            _webElement = new WebElement(browser);
            _browser.FrameLoadEnd += OnBrowserOnFrameLoadEnd;

            var requestHandlerInterceptionFactory = new RequestHandlerInterceptionFactory();

            requestHandlerInterceptionFactory.Register("cc.skype.com/cc/v1", new CallSignalingInterceptor());
            requestHandlerInterceptionFactory.Register("gateway.messenger.live.com/v1", new PollingMessageInterceptor());

            _browser.RequestHandler = requestHandlerInterceptionFactory;
        }

        private void OnBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                Console.WriteLine($"Page load completed: {e.Url}");

                if (e.Url.Equals(SkypeWebAppUrl)) Console.WriteLine("Application Loaded");
            }
        }

        public void Login(string user, string password)
        {
            var callerDispatcher = Dispatcher.CurrentDispatcher;

            Task.Run(async () =>
            {
                try
                {
                    bool waitForInitialization = true;
                    while (waitForInitialization)
                    {
                        bool isInitialized = false;
                        await callerDispatcher.InvokeAsync(() => isInitialized = _browser.IsBrowserInitialized);

                        if (Cef.IsInitialized && isInitialized)
                        {
                            waitForInitialization = false;
                        }

                        await Task.Delay(10);

                    }

                    _browser.Load(SkypeWebAppUrl);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                await _webElement.SetElementTextByName("loginfmt", user);
                await _webElement.ClickButtonById("idSIButton9");

                await _webElement.SetElementTextByName("passwd", password);
                await _webElement.ClickButtonById("idSIButton9");
            });
        }
    }
}