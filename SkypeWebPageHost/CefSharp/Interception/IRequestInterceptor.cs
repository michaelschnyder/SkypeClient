using System.IO;
using CefSharp;

namespace SkypeWebPageHost.CefSharp.Interception
{
    public interface IRequestInterceptor
    {
        void Execute(IResponse response, MemoryStream stream);
    }
}