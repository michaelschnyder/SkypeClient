using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CefSharp;
using CefSharp.Extensions.Interception;
using Skype.Client.Channel;

namespace Skype.Client.CefSharp
{
    public class ChannelForwardResponseInterceptor : IResponseInterceptor
    {
        private readonly MessageChannel _messageChannel;

        public ChannelForwardResponseInterceptor(MessageChannel messageChannel)
        {
            _messageChannel = messageChannel;
        }

        public void Execute(IRequest request, IResponse response, MemoryStream stream)
        {
            if (response.Charset != "utf-8" || stream.Length == 0) return;

            var str = Encoding.UTF8.GetString(stream.ToArray());
            // _messageChannel.PublishMessage(str);

            var channelRequest = new Channel.Request
            {
                Uri = new Uri(request.Url)
            };

            var channelResponse = new Channel.Response()
            {
                Content = str,
                Headers = response.Headers
            };

            _messageChannel.PublishMessage(channelRequest, channelResponse);
        }
    }

}