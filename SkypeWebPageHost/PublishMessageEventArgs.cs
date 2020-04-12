using System;

namespace SkypeWebPageHost
{
    public class PublishMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public PublishMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}