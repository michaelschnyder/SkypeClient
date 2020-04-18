using System;
using Newtonsoft.Json;
using Skype.Client.Protocol.Events;

namespace Skype.Client.Protocol.Conversation
{
    class ConversationsFrame
    {

        public Conversation[] conversations { get; set; }
        
        [JsonProperty("_metadata")]
        public Metadata Metadata { get; set; }

    }

    public class Conversation
    {
        public string targetLink { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public long version { get; set; }
        public Properties properties { get; set; }
        public Lastmessage lastMessage { get; set; }
        public string messages { get; set; }
    }

    public class Properties
    {
        public string isemptyconversation { get; set; }
        public string consumptionhorizon { get; set; }
        public string onetoonethreadid { get; set; }
        public string conversationstatus { get; set; }
        public DateTime lastimreceivedtime { get; set; }
    }

    public class Lastmessage
    {
        public string id { get; set; }
        public DateTime originalarrivaltime { get; set; }
        public string messagetype { get; set; }
        public string version { get; set; }
        public DateTime composetime { get; set; }
        public string clientmessageid { get; set; }
        public string conversationLink { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public string conversationid { get; set; }
        public string from { get; set; }
    }

}
