using Newtonsoft.Json;
using Skype.Client.Protocol.Events.Resource;

namespace Skype.Client.Protocol.Events
{
    public class EventMessageFrame
    {
        [JsonProperty("eventmessages")]
        public EventMessage[] EventMessages { get; set; }
    }
}