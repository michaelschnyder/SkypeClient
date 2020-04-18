namespace Skype.Client.Protocol.Events.Resource
{
    public class EndpointPresenceResource : BaseResource
    {
        public string SelfLink { get; set; }
        public PublicInfo PublicInfo { get; set; }
        public PrivateInfo PrivateInfo { get; set; }

    }

    public class PublicInfo
    {
        public string Capabilities { get; set; }
        public string Typ { get; set; }
        public string SkypeNameVersion { get; set; }
        public string NodeInfo { get; set; }
        public string Version { get; set; }
    }

    public class PrivateInfo
    {
        public string EpName { get; set; }
    }

}