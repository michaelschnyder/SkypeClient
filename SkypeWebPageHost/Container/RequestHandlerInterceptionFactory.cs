using System.Collections.Generic;
using System.Linq;
using CefSharp;
using CefSharp.Handler;

namespace SkypeWebPageHost.Container
{
    public class RequestHandlerInterceptionFactory : RequestHandler
    {
        private Dictionary<string, List<IRequestInterceptor>> interceptors = new Dictionary<string, List<IRequestInterceptor>>();

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            var list = this.interceptors.Where(kvp => request.Url.Contains(kvp.Key)).SelectMany(kvp => kvp.Value).ToList();

            if (list.Any())
            {
                return new DelegatingResourceRequestHandler((response, stream) =>
                {
                    foreach (var requestInterceptor in list)
                    {
                        requestInterceptor.Execute(response, stream);
                    }
                });
            }

            return null;
        }

        public void Register(string url, IRequestInterceptor requestInterceptor)
        {

            if (this.interceptors.ContainsKey(url))
            {
                this.interceptors[url].Add(requestInterceptor);
            }
            else
            {
                this.interceptors.Add(url, new List<IRequestInterceptor>() { requestInterceptor });
            }
        }
    }
}