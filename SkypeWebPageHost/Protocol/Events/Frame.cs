using Newtonsoft.Json;

namespace SkypeWebPageHost.Protocol.Events
{
    public class Frame
    {
        [JsonProperty("eventmessages")]
        public EventMessage[] EventMessages { get; set; }

        [JsonProperty("messages")]
        public SyncMessage[] SyncMessages { get; set; }

        public Response[] Responses { get; set; }

        [JsonProperty("_metadata")]
        public Metadata Metadata { get; set; }

        public int ErrorCode { get; set; }
        public string Message { get; set; }

    }

    public class Metadata
    {
        public string SyncState { get; set; }
        public string BackwardLink { get; set; }
        public long LastCompleteSegmentStartTime { get; set; }
        public long LastCompleteSegmentEndTime { get; set; }
    }

}