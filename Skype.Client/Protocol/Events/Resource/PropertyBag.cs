using Newtonsoft.Json;
using Skype.Client.Protocol.Events.Resource.Properties;

namespace Skype.Client.Protocol.Events.Resource
{
    public class PropertyBag
    {
        [JsonProperty("call-log")]
        [JsonConverter(typeof(EmbeddedJsonConverter))]
        public CallLog CallLog { get; set; }
    }
}