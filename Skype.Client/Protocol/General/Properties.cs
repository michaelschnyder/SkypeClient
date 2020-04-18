using System;
using System.Collections.Generic;
using System.Text;

namespace Skype.Client.Protocol.General
{
    public class Properties
    {
        public string market { get; set; }
        public string readReceiptsEnabled { get; set; }
        public string timezone { get; set; }
        public string isSetupWizardCompleted { get; set; }
        public string scheduleNextCallCurrentSuccessfulCalls { get; set; }
        public string locale { get; set; }
        public string alertsLastReadTime { get; set; }
        public string coachMarksSeen { get; set; }
        public string lastActivityAt { get; set; }
        public string lastActivityAt2 { get; set; }
        public string userRing { get; set; }
        public string isEncryptionCapable { get; set; }
        public string cnsLongPollEnabled { get; set; }
        public string cnsPushEnabled { get; set; }
        public string cid { get; set; }
        public string cidHex { get; set; }
        public bool dogfoodUser { get; set; }
        public string primaryMemberName { get; set; }
        public string skypeName { get; set; }
    }
}
