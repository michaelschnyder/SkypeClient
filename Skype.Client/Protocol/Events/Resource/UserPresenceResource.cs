namespace Skype.Client.Protocol.Events.Resource
{
    public class UserPresenceResource : BaseResource
    {
        public string SelfLink { get; set; }
        public string Availability { get; set; }
        public string Status { get; set; }
        public string Capabilities { get; set; }
        public string[] EndpointPresenceDocLinks { get; set; }
    }
}