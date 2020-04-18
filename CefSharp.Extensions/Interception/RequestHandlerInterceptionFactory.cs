using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CefSharp.Handler;

namespace CefSharp.Extensions.Interception
{
    public class RequestHandlerInterceptionFactory : RequestHandler
    {
        private Dictionary<Regex, List<IRequestInterceptor>> interceptors = new Dictionary<Regex, List<IRequestInterceptor>>();

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

        public void Register(string url, IRequestInterceptor requestInterceptor)
        {
            Register(new Regex(url), requestInterceptor);
        }

        public void Register(Regex urlPattern, IRequestInterceptor requestInterceptor)
        {

            if (this.interceptors.ContainsKey(urlPattern))
            {
                this.interceptors[urlPattern].Add(requestInterceptor);
            }
            else
            {
                this.interceptors.Add(urlPattern, new List<IRequestInterceptor>() { requestInterceptor });
            }
        }
    }
}