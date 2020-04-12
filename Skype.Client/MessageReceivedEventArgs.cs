namespace Skype.Client
{
    public class MessageReceivedEventArgs
    {
        public string SenderName { get; set; }

        public string MessageHtml { get; set; }

        public override string ToString()
        {
            return $"New Message from '{SenderName}'. Content: '{MessageHtml}'";
        }
    }
}