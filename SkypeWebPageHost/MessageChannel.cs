using System;

namespace SkypeWebPageHost
{
    public class MessageChannel
    {
        public event EventHandler<PublishMessageEventArgs> MessagePublished;

        public void PublishMessage(string message)
        {
            this.OnMessagePublished(new PublishMessageEventArgs(message));
        }

        protected virtual void OnMessagePublished(PublishMessageEventArgs e)
        {
            MessagePublished?.Invoke(this, e);
        }
    }
}