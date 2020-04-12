using System;
using Newtonsoft.Json;
using SkypeWebPageHost.Protocol.Events.Resource.Properties;

namespace SkypeWebPageHost.Protocol.Events.Resource
{
    public class Resource
    {
        public string ConversationLink { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string ClientMessageId { get; set; }
        public string ReceiverDisplayName { get; set; }
        public string Version { get; set; }
        public string MessageType { get; set; }
        public string CounterPartyMessageId { get; set; }
        public string ImDisplayName { get; set; }
        public string Content { get; set; }
        public DateTime ComposeTime { get; set; }
        public string OriginContextId { get; set; }
        public DateTime OriginalArrivalTime { get; set; }
        public string AckRequired { get; set; }
        public string ContentType { get; set; }
        public string SkypeGuid { get; set; }
        public bool IsActive { get; set; }
        public string Id { get; set; }

        public string CallerDisplayName { get; set; }
        public string IsVideoCall { get; set; }
        public string ConversationId { get; set; }
        public string EventId { get; set; }
        public string Expiration { get; set; }
        public string S2SPartnerName { get; set; }

        public PropertyBag Properties { get; set; }
    }

    public class PropertyBag
    {
        [JsonProperty("call-log")]
        [JsonConverter(typeof(EmbeddedJsonConverter))]
        public CallLog CallLog { get; set; }
    }
}