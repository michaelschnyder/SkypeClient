using System;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Extensions;
using CefSharp.Extensions.Interception;
using CefSharp.Internals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Skype.Client.CefSharp
{
    public class SkypeCefClient : SkypeClient
    {
        private const string SkypeWebAppUrl = "https://web.skype.com/";
        private const string CallSignalingUrl = "cc.skype.com/cc/v1";
        private const string EventMessageUrl = "gateway.messenger.live.com/v1";

        private readonly PageInteraction _pageInteraction;
        private ILogger _logger;
        protected IRenderWebBrowser RenderWebBrowser { get; }

        public SkypeCefClient(IRenderWebBrowser browser) : this(browser, NullLoggerFactory.Instance)
        {
        }

        public SkypeCefClient(IRenderWebBrowser browser, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(SkypeCefClient));
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
            _logger.LogDebug("Navigation started to url '{url}'", e.Frame.Url);
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

                    _logger.LogDebug("Login started. Waiting for CefBrowser to be initialized");
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
                    _logger.LogDebug("CefBrowser Initialized. Navigating to login page {loginPage}", SkypeWebAppUrl);

                    RenderWebBrowser.Load(SkypeWebAppUrl);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error happened while awaiting initialization phase of CefBrowser");
                    throw;
                }

                
                _logger.LogDebug("Filling in user '{user}' to login form", user);
                await _pageInteraction.SetElementTextByName("loginfmt", user);

                _logger.LogDebug("Continue to password page by clicking button");
                await _pageInteraction.ClickButtonById("idSIButton9");

                _logger.LogDebug("Filling in password to login form");
                await _pageInteraction.SetElementTextByName("passwd", password);

                _logger.LogDebug("Complete login flow by clicking button");
                await _pageInteraction.ClickButtonById("idSIButton9");
            });
        }
    }
}