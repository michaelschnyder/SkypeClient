using System.IO;

namespace CefSharp.Extensions.Interception
{
    public interface IResponseInterceptor
    {
        void Execute(IRequest request, IResponse response, MemoryStream stream);
    }
}