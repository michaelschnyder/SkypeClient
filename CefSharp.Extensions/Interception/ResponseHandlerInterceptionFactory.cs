using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CefSharp.Handler;

namespace CefSharp.Extensions.Interception
{
    public class ResponseHandlerInterceptionFactory : RequestHandler
    {
        private Dictionary<Regex, List<IResponseInterceptor>> interceptors = new Dictionary<Regex, List<IResponseInterceptor>>();

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            var list = this.interceptors.Where(kvp => kvp.Key.IsMatch(request.Url)).SelectMany(kvp => kvp.Value).ToList();

            if (list.Any())
            {
                return new DelegatingResourceRequestHandler((response, stream) =>
                {
                    foreach (var requestInterceptor in list)
                    {
                        requestInterceptor.Execute(request, response, stream);
                    }
                });
            }

            return null;
        }

        public void Register(string url, IResponseInterceptor responseInterceptor)
        {
            Register(new Regex(url), responseInterceptor);
        }

        public void Register(Regex urlPattern, IResponseInterceptor responseInterceptor)
        {

            if (this.interceptors.ContainsKey(urlPattern))
            {
                this.interceptors[urlPattern].Add(responseInterceptor);
            }
            else
            {
                this.interceptors.Add(urlPattern, new List<IResponseInterceptor>() { responseInterceptor });
            }
        }
    }
}