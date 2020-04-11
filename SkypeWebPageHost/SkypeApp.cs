using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using SkypeWebPageHost.Container;

namespace SkypeWebPageHost
{
    public class SkypeApp
    {
        private static string SkypeWebAppUrl = "https://web.skype.com/";

        private readonly ChromiumWebBrowser _browser;
        private readonly WebElement _webElement;

        public SkypeApp(ChromiumWebBrowser browser)
        {
            _browser = browser;
            _webElement = new WebElement(browser);
            _browser.FrameLoadEnd += OnBrowserOnFrameLoadEnd;

            _browser.RequestHandler = new RequestHandlerInterceptionFactory();
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
            _browser.Address = SkypeWebAppUrl;

            Task.Run(async () =>
            {
                await _webElement.SetElementTextByName("loginfmt", user);
                await _webElement.ClickButtonById("idSIButton9");

                await _webElement.SetElementTextByName("passwd", password);
                await _webElement.ClickButtonById("idSIButton9");
            });
        }
    }
}