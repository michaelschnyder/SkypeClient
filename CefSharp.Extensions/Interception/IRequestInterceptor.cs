using System.IO;

namespace CefSharp.Extensions.Interception
{
    public interface IRequestInterceptor
    {
        void Execute(IRequest request, IResponse response, MemoryStream stream);
    }
}