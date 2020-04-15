using System;
using Microsoft.Extensions.Logging;

namespace Skype.Client.Channel
{
    public class MessageChannel
    {
        private readonly ILogger _logger;

        public MessageChannel(ILogger logger)
        {
            _logger = logger;
        }
        public event EventHandler<PublishMessageEventArgs> MessagePublished;

        public void PublishMessage(string message)
        {
            _logger.LogDebug("Incoming Message on channel: {rawMessage}", message);
            this.OnMessagePublished(new PublishMessageEventArgs(message));
        }

        protected virtual void OnMessagePublished(PublishMessageEventArgs e)
        {
            MessagePublished?.Invoke(this, e);
        }
    }
}