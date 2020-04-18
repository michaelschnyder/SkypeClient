using System;
using System.Collections.Generic;
using System.Text;

namespace Skype.Client.Protocol.Events.Resource
{
    public class CustomUserPropertiesResource : BaseResource
    {
        public string CoachMarksSeen { get; set; }

        public string ScheduleNextCallCurrentSuccessfulCalls { get; set; }
    }
}
