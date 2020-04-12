using System;

namespace Skype.Client.Channel
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