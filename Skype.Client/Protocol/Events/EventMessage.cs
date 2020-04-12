using System;

namespace Skype.Client.Protocol.Events
{
    public class EventMessage
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string ResourceType { get; set; }
        public DateTime Time { get; set; }
        public string ResourceLink { get; set; }
        public Resource.Resource Resource { get; set; }
    }
}