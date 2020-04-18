using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Skype.Client.Protocol.Events.Resource;

namespace Skype.Client.Protocol.Events
{
    [JsonConverter(typeof(ResourceTypeConverter))]
    public class EventMessage
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string ResourceType { get; set; }
        public DateTime Time { get; set; }
        public string ResourceLink { get; set; }

        public BaseResource Resource { get; set; }
    }

 
    public class ResourceTypeConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var thisJObject = JObject.Load(reader);

            var typeName = "Unknown";
            JToken result;
            
            if (thisJObject.TryGetValue("ResourceType", StringComparison.InvariantCultureIgnoreCase, out result))
            {
                typeName = result.ToString();
            }

            var message = new EventMessage();

            switch (typeName.ToLowerInvariant())
            {
                case "endpointpresence":
                    message.Resource = new EndpointPresenceResource();
                    break;

                case "newmessage":
                    message.Resource = new NewMessageResource();
                    break;

                case "userpresence":
                    message.Resource = new UserPresenceResource();
                    break;

                case "customuserproperties":
                    message.Resource = new CustomUserPropertiesResource();
                    break;

                default:
                    message.Resource = new BaseResource();
                    break;
            }

            // Populate the object properties
            serializer.Populate(thisJObject.CreateReader(), message);

            return message;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}