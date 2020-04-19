using System;

namespace Skype.Client.Channel
{
    public class PublishMessageEventArgs : EventArgs
    {
        public Request Request { get; }
        public Response Response { get; }
        public string Message { get; }

        public PublishMessageEventArgs(string message)
        {
            Message = message;
        }

        public PublishMessageEventArgs(Request request, Response response)
        {
            Request = request;
            Response = response;
            Message = response.Content;
        }
    }
}