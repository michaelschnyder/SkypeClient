using System.IO;
using System.Text;
using CefSharp;
using CefSharp.Extensions.Interception;
using SkypeWebPageHost;

namespace Skype.Client.CefSharp
{
    public class ChannelForwardInterceptor : IRequestInterceptor
    {
        private readonly MessageChannel _messageChannel;

        public ChannelForwardInterceptor(MessageChannel messageChannel)
        {
            _messageChannel = messageChannel;
        }

        public void Execute(IResponse response, MemoryStream stream)
        {
            if (response.Charset != "utf-8" || stream.Length == 0) return;

            var str = Encoding.UTF8.GetString(stream.ToArray());
            _messageChannel.PublishMessage(str);
        }
    }
}