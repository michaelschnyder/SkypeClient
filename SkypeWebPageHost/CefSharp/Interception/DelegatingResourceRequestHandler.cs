using System;
using System.IO;
using CefSharp;
using CefSharp.Handler;

namespace SkypeWebPageHost.CefSharp.Interception
{
    public class DelegatingResourceRequestHandler : ResourceRequestHandler
    {
        private readonly Action<IResponse, MemoryStream> _response;
        private readonly MemoryStream _memoryStream = new MemoryStream();

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return new global::CefSharp.ResponseFilter.StreamResponseFilter(_memoryStream);
        }

        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            _response(response, _memoryStream);
        }

        public DelegatingResourceRequestHandler(Action<IResponse, MemoryStream> response)
        {
            _response = response;
        }
    }
}