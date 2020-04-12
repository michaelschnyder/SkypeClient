using System;

namespace Skype.Client.Protocol.Events.Resource.Properties
{
    public class CallLog
    {
        public DateTime StartTime { get; set; }
        public object ConnectTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CallDirection { get; set; }
        public string CallType { get; set; }
        public string CallState { get; set; }
        public string Originator { get; set; }
        public string Target { get; set; }
        public Participant OriginatorParticipant { get; set; }
        public Participant TargetParticipant { get; set; }
        public string CallId { get; set; }
        public object CallAttributes { get; set; }
        public object ForwardingInfo { get; set; }
        public object TransferInfo { get; set; }
        public object Participants { get; set; }
        public object ParticipantList { get; set; }
        public object ThreadId { get; set; }
        public string SessionType { get; set; }
        public object MessageId { get; set; }
    }

    public class Participant
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
    }

}