using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Skype.Client.Protocol.Events;

namespace Skype.Client.Protocol.Conversation { 

    public class MessagesFrame
    {
        public Message[] Messages { get; set; }

        [JsonProperty("_metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Message
    {
        public string Id { get; set; }
        public DateTime OriginalArrivalTime { get; set; }
        public string MessageType { get; set; }
        public string Version { get; set; }
        public DateTime ComposeTime { get; set; }
        public string ClientMessageId { get; set; }
        public string ConversationLink { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public string ConversationId { get; set; }
        public string From { get; set; }
        public string SkypeGuid { get; set; }
        public string SkypeEditedId { get; set; }

    }
}
