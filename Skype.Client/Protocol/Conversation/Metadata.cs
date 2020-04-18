namespace Skype.Client.Protocol.Conversation
{
    public class Metadata
    {
        public string SyncState { get; set; }
        public string BackwardLink { get; set; }
        public long LastCompleteSegmentStartTime { get; set; }
        public long LastCompleteSegmentEndTime { get; set; }
    }
}