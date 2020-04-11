using System.IO;

namespace CefSharp.Extensions.Interception
{
    public interface IRequestInterceptor
    {
        void Execute(IResponse response, MemoryStream stream);
    }
}