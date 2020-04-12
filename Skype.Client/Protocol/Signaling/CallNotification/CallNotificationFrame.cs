namespace Skype.Client.Protocol.Signaling.CallNotification
{

    public class CallNotificationFrame
    {
        public Participants Participants { get; set; }
        public CallInvitation CallInvitation { get; set; }
        public AdditionalActionResponse[] AdditionalActionResponses { get; set; }
        public DebugContent DebugContent { get; set; }
    }

    public class Participants
    {
        public Participant From { get; set; }
        public Participant To { get; set; }
    }

    public class Participant
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string EndpointId { get; set; }
        public string LanguageId { get; set; }
        public string ParticipantId { get; set; }
        public bool Hidden { get; set; }
       
    }

    public class CallInvitation
    {
        public string[] CallModalities { get; set; }
        public Links Links { get; set; }
        public object MediaContent { get; set; }
    }

    public class Links
    {
        public string Progress { get; set; }
        public string NewOffer { get; set; }
        public string MediaAnswer { get; set; }
        public string Acceptance { get; set; }
        public string Redirection { get; set; }
        public string CallController { get; set; }
        public string CallLeg { get; set; }
        public string Subscribe { get; set; }
        public string BrokerHttpTransport { get; set; }
    }

    public class DebugContent
    {
        public string CallId { get; set; }
        public string ParticipantId { get; set; }
    }
}
