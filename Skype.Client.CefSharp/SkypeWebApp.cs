using System;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Extensions;
using CefSharp.Extensions.Interception;
using CefSharp.Internals;
using SkypeWebPageHost;

namespace Skype.Client.CefSharp
{
    public class SkypeWebApp
    {
        private static string SkypeWebAppUrl = "https://web.skype.com/";

        private readonly IRenderWebBrowser _browser;
        private readonly PageInteraction _pageInteraction;
        
        private MessageChannel _callSignalingChannel = new MessageChannel();
        private MessageChannel _eventChannel = new MessageChannel();

        public SkypeWebApp(IRenderWebBrowser browser)
        {
            _browser = browser;
            _pageInteraction = new PageInteraction(browser);
            _browser.FrameLoadEnd += OnBrowserOnFrameLoadEnd;

            var requestHandlerInterceptionFactory = new RequestHandlerInterceptionFactory();

            requestHandlerInterceptionFactory.Register("cc.skype.com/cc/v1", new ChannelForwardInterceptor(this._callSignalingChannel));
            requestHandlerInterceptionFactory.Register("gateway.messenger.live.com/v1", new ChannelForwardInterceptor(this._eventChannel));

            _browser.RequestHandler = requestHandlerInterceptionFactory;

            var skypeClient = new SkypeClient(this._callSignalingChannel, this._eventChannel);
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
            var ctx = SynchronizationContext.Current;

            Task.Run(async () =>
            {
                try
                {
                    bool waitForInitialization = true;
                    bool isInitialized = false;

                    while (waitForInitialization)
                    {
                        ctx.Post(state => isInitialized = _browser.IsBrowserInitialized, null);

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

                await _pageInteraction.SetElementTextByName("loginfmt", user);
                await _pageInteraction.ClickButtonById("idSIButton9");

                await _pageInteraction.SetElementTextByName("passwd", password);
                await _pageInteraction.ClickButtonById("idSIButton9");
            });
        }
    }
}