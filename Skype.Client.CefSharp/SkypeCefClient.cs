using System;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Extensions;
using CefSharp.Extensions.Interception;
using CefSharp.Internals;

namespace Skype.Client.CefSharp
{
    public class SkypeCefClient : SkypeClient
    {
        private const string SkypeWebAppUrl = "https://web.skype.com/";
        private const string CallSignalingUrl = "cc.skype.com/cc/v1";
        private const string EventMessageUrl = "gateway.messenger.live.com/v1";

        private readonly PageInteraction _pageInteraction;
        protected IRenderWebBrowser RenderWebBrowser { get; }

        public SkypeCefClient(IRenderWebBrowser browser)
        {
            RenderWebBrowser = browser;

            _pageInteraction = new PageInteraction(browser);
            RenderWebBrowser.FrameLoadStart += RenderWebBrowserOnFrameLoadStart;

            var requestHandlerInterceptionFactory = new RequestHandlerInterceptionFactory();

            requestHandlerInterceptionFactory.Register(CallSignalingUrl, new ChannelForwardInterceptor(CallSignalingChannel));
            requestHandlerInterceptionFactory.Register(EventMessageUrl, new ChannelForwardInterceptor(EventChannel));

            RenderWebBrowser.RequestHandler = requestHandlerInterceptionFactory;
        }

        private void RenderWebBrowserOnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            if (Status == AppStatus.Authenticating && e.Frame.Url == SkypeWebAppUrl)
            {
                this.UpdateStatus(AppStatus.Authenticated);
                this.UpdateStatus(AppStatus.Loading);
            }
        }

        public void Login(string user, string password)
        {
            var ctx = SynchronizationContext.Current;

            Task.Run(async () =>
            {
                try
                {
                    this.UpdateStatus(AppStatus.Initializing);

                    bool waitForInitialization = true;
                    bool isInitialized = false;

                    while (waitForInitialization)
                    {
                        if (ctx != null)
                        {
                            ctx.Post(state => isInitialized = RenderWebBrowser.IsBrowserInitialized, null);
                        }
                        else
                        {
                            isInitialized = RenderWebBrowser.IsBrowserInitialized;
                        }

                        if (Cef.IsInitialized && isInitialized)
                        {
                            waitForInitialization = false;
                        }

                        await Task.Delay(10);

                    }

                    this.UpdateStatus(AppStatus.Authenticating);
                    RenderWebBrowser.Load(SkypeWebAppUrl);
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