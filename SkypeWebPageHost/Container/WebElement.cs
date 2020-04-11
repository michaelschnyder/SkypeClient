using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;

namespace SkypeWebPageHost.Container
{
    public class WebElement
    {
        private readonly ChromiumWebBrowser _browser;
        private bool _pageReady;

        public WebElement(ChromiumWebBrowser browser)
        {
            _browser = browser;

            _browser.FrameLoadEnd += OnBrowserOnFrameLoadEnd;
            _browser.FrameLoadStart += (sender, e) => _pageReady = false;
        }

        private void OnBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                _pageReady = true;
            }
        }

        public async Task ClickButtonById(string id)
        {
            await WaitForElementById(id);

            var response = await _browser.GetMainFrame().EvaluateScriptAsync(
                "(function() { " +
                $"  document.getElementById('{id}').focus();" +
                $"  return document.getElementById('{id}').click();" +
                "})()");

            if (response.Success)
            {
                Console.WriteLine("button clicked.");
            }

        }

        public async Task SetElementTextByName(string name, string value)
        {
            await WaitForElementByName(name);

            var response = await _browser.GetMainFrame().EvaluateScriptAsync(
                "(function() { " +
                $"  document.getElementsByName('{name}')[0].focus();" +
                $"  document.getElementsByName('{name}')[0].value = '{value}'; " +
                $"  return document.getElementsByName('{name}')[0].value == '{value}';" +
                "})()");

            if (response.Success && (bool)response.Result)
            {
                Console.WriteLine("text set.");
            }
        }

        private async Task WaitForElementByName(string name)
        {
            var queryJs = $"document.getElementsByName('{name}')[0]";

            await ResolveElement(queryJs);
        }

        private async Task WaitForElementById(string name)
        {
            var queryJs = $"document.getElementById('{name}')";

            await ResolveElement(queryJs);
        }

        private async Task ResolveElement(string resolver)
        {
            var script = $"(function() {{ " +
                         $"var o = {resolver};" +
                         $"if (o == undefined) return false;" +
                         $"if (o.attributes[\"aria-hidden\"] != undefined && o.attributes[\"aria-hidden\"].value == 'true') return false;" +
                         $"return true;" +

                         $" }})()";

            while (true)
            {
                if (!_pageReady)
                {
                    await Task.Delay(10);
                    continue;
                }

                var response = await _browser.GetMainFrame().EvaluateScriptAsync(script);

                if (response.Success && (bool)response.Result == true)
                {
                    Console.WriteLine("Element found.");
                    return;
                }

                Console.WriteLine($"Missing element. Execution Status: {response.Success}, Result: {response.Result}, Message: {response.Result}");
            }
        }
    }
}