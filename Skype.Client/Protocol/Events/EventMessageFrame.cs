using Newtonsoft.Json;

namespace Skype.Client.Protocol.Events
{
    public class EventMessageFrame
    {
        [JsonProperty("eventmessages")]
        public EventMessage[] EventMessages { get; set; }
    }
}