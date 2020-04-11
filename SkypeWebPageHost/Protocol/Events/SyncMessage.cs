using System;

namespace SkypeWebPageHost.Protocol.Events
{
    public class SyncMessage
    {
        public string Id { get; set; }
        public DateTime OriginalArrivalTime { get; set; }
        public string MessageType { get; set; }
        public string Version { get; set; }
        public DateTime ComposeTime { get; set; }
        public string ClientMessageid { get; set; }
        public string SkypeGuid { get; set; }
        public string Content { get; set; }
        public string ConversationLink { get; set; }
        public string ConversationId { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string SkypeEditedId { get; set; }
    }
}