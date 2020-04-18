using System;
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
    }
}