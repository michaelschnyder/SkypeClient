using System;

namespace SkypeWebPageHost.Protocol.Events
{
    public class Response
    {
        public int Status { get; set; }
        public string Contact { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string SelfLink { get; set; }
        public string Availability { get; set; }
        public string Status { get; set; }
        public string Capabilities { get; set; }
        public DateTime LastSeenAt { get; set; }
        public string[] EndpointPresenceDocLinks { get; set; }
    }


}