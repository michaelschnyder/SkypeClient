using System.IO;
using CefSharp;

namespace SkypeWebPageHost.CefSharpExtensions.Interception
{
    public interface IRequestInterceptor
    {
        void Execute(IResponse response, MemoryStream stream);
    }
}