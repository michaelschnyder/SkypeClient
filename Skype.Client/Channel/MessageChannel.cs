using System;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace Skype.Client.Channel
{
    public class MessageChannel
    {
        private readonly string _name;
        private readonly ILogger _logger;

        public MessageChannel(string name, ILogger logger)
        {
            _name = name;
            _logger = logger;
        }
        public event EventHandler<PublishMessageEventArgs> MessagePublished;

        public void PublishMessage(string message)
        {
            _logger.LogDebug("Incoming Message on channel '{name}': {rawMessage}", _name, message);
            this.OnMessagePublished(new PublishMessageEventArgs(message));
        }

        protected virtual void OnMessagePublished(PublishMessageEventArgs e)
        {
            MessagePublished?.Invoke(this, e);
        }

        public void PublishMessage(Request request, Response response)
        {
            _logger.LogDebug("Response on channel '{name}': {rawMessage}", _name, response.Content);
            this.OnMessagePublished(new PublishMessageEventArgs(request, response));
        }
    }

    public class Response
    {
        public string Content { get; set; }

        public NameValueCollection Headers { get; set; }
    }

    public class Request
    {
        public Uri Uri { get; set; }
    }
}