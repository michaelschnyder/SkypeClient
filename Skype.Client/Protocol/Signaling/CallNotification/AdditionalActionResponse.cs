using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skype.Client.Protocol.Signaling.CallNotification
{
    public class AdditionalActionResponse
    {
        public string Url { get; set; }
        public Output Output { get; set; }
    }

    public class Output
    {
        public Roster Roster { get; set; }
        public string ConversationController { get; set; }
        public int SequenceNumber { get; set; }
        public string Subject { get; set; }
        public ActiveModalities ActiveModalities { get; set; }
        public State State { get; set; }
        public CallControlLinks Links { get; set; }
        public Capabilities Capabilities { get; set; }
    }

    public class Roster
    {
        [JsonProperty("participants")]
        public Dictionary<string, ParticipantItem> Participants { get; set; }

        public string Type { get; set; }
        public int SequenceNumber { get; set; }
    }

    public class ParticipantItem
    {
        public string ParticipantLink { get; set; }
        public Participant Details { get; set; }
        public Dictionary<string, Endpoint> Endpoints { get; set; }
        public string Role { get; set; }
    }

    public class Endpoint
    {
        public Call Call { get; set; }
        public Capabilities Capabilities { get; set; }
        public string ParticipantId { get; set; }
        public string ClientVersion { get; set; }
        public EndpointMetadata EndpointMetadata { get; set; }
    }

    public class Call
    {
        public int ServerMuteVersion { get; set; }
    }

    public class Capabilities
    {
        public string CloudAudioVideoConference { get; set; }
        public string CloudScreenSharing { get; set; }
        public string HostlessConference { get; set; }
        public string CloudMerge { get; set; }
        public string AdditionalModalityOperationLinks { get; set; }
        public string ImplicitCallback { get; set; }
        public string AutoJoinOnConflict { get; set; }
        public string SupportsCompressedServicePayload { get; set; }
        public string ServerMuteUnmute { get; set; }
        public string SupportNgcMediaControl { get; set; }
    }

    public class EndpointMetadata
    {
        public string CallTranslatorBotSpokenLanguageLocale { get; set; }
        public bool CallTranslatorBotVoiceCollectionAllowed { get; set; }
        public bool IsCallMediaCaptured { get; set; }
        public bool ReactionsEnabled { get; set; }
        public bool SupportsCallRecording { get; set; }
        public bool SupportsNotifyCallMediaCaptured { get; set; }
    }

    public class ActiveModalities
    {
        public object Call { get; set; }
    }

    public class State
    {
        public bool IsMultiParty { get; set; }
        public object GroupCallInitiator { get; set; }
        public bool IsBroadcast { get; set; }
        public bool IsVoiceDataCollectionOn { get; set; }
    }

    public class CallControlLinks
    {
        public string Leave { get; set; }
        public string AddParticipant { get; set; }
        public string RemoveParticipant { get; set; }
        public string AddModality { get; set; }
        public string AddParticipantAndModality { get; set; }
        public string RemoveModality { get; set; }
        public string Mute { get; set; }
        public string Unmute { get; set; }
        public string NotificationLinks { get; set; }
        public string Merge { get; set; }
        public string UpdateEndpointMetadata { get; set; }
        public string UpdateEndpointState { get; set; }
        public string Admit { get; set; }
        public string ConversationHttpTransport { get; set; }
        public string PublishState { get; set; }
        public string RemoveState { get; set; }
        public string UpdateMeetingSettings { get; set; }
    }
}
