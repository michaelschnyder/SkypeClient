using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Extensions;
using CefSharp.Extensions.Interception;
using CefSharp.Internals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skype.Client.Channel;

namespace Skype.Client.CefSharp
{
    public class SkypeCefClient : SkypeClient
    {
        private const string SkypeWebAppUrl = "https://web.skype.com/";
        private const string CallSignalingUrl = "cc.skype.com/cc/v1";

        // Event channel
        // GET Request URL: https://client-s.gateway.messenger.live.com/v1/users/ME/endpoints/???/subscriptions/0/poll?ackId=12345
        private static readonly Regex EventMessageUrlPattern = new Regex(@"https://.*messenger.live.com/v1/.*/poll");

        // Publish active status
        // POST https://azseas1-client-s.gateway.messenger.live.com/v1/users/ME/endpoints/???/active
        private static readonly Regex OwnStatusUpdateUrlPattern = new Regex(@"https://.*.messenger.live.com/v1/users/ME/endpoints/.*/active");
        
        // Own user presence doc
        // GET https://azseas1-client-s.gateway.messenger.live.com/v1/users/ME/presenceDocs/messagingService
        private static readonly Regex OwnPresenceStatusUrlPattern = new Regex(@"https://.*messenger.live.com/v1/users/ME/presenceDocs/messagingService");

        // Other User presence doc: https://client-s.gateway.messenger.live.com/v1/users/ME/contacts/ALL/presenceDocs/messagingService?cMri=...
        private static readonly Regex ContactsUserPresenceUrlPattern = new Regex(@"https://.*messenger.live.com/v1/users/ME/contacts/ALL/presenceDocs/messagingService\?cMri=.*");

        // Active Conversations
        // GET https://azseas1-client-s.gateway.messenger.live.com/v1/users/ME/conversations?view=supportsExtendedHistory%7Cmsnp24Equivalent&pageSize=25&syncState=...
        private static readonly Regex ConversationsUrlPattern = new Regex(@"https://.*messenger.live.com/v1/users/ME/conversations\?");

        // Past Messages for conversations
        // GET https://azseas1-client-s.gateway.messenger.live.com/v1/users/ME/conversations/.../messages?view=supportsExtendedHistory%7Cmsnp24Equivalent%7CsupportsMessageProperties&pageSize=20&syncState=...&startTime=...
        private static readonly Regex PastMessagesUrlPattern = new Regex(@"https://.*messenger.live.com/v1/users/ME/conversations/.*/messages.*");

        // Properties
        // GET https://azseas1-client-s.gateway.messenger.live.com/v1/users/ME/properties
        private static readonly Regex OwnPropertiesUrlPattern = new Regex(@"https://.*messenger.live.com/v1/users/ME/properties");

        // Own profile
        // POST https://people.skype.com/v2/profiles
        private static readonly Regex ProfilesUrlPattern = new Regex(@"https://people.skype.com/v2/profiles");

        // Contacts
        // Request URL: https://edge.skype.com/pcs/contacts/v2/users/self
        private static readonly Regex ContactsUrlPattern = new Regex(@"https://edge.skype.com/pcs/contacts/v2/users/self");

        private readonly PageInteraction _pageInteraction;
        private readonly ILogger _logger;

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
            requestHandlerInterceptionFactory.Register(EventMessageUrlPattern, new ChannelForwardInterceptor(EventChannel));
            requestHandlerInterceptionFactory.Register(ContactsUserPresenceUrlPattern, new ChannelForwardInterceptor(UserPresenceChannel));
            requestHandlerInterceptionFactory.Register(OwnPropertiesUrlPattern, new ChannelForwardInterceptor(PropertiesChannel));
            requestHandlerInterceptionFactory.Register(ProfilesUrlPattern, new ChannelForwardInterceptor(ProfilesChannel));
            requestHandlerInterceptionFactory.Register(ContactsUrlPattern, new ChannelForwardInterceptor(ContactsChannel));

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