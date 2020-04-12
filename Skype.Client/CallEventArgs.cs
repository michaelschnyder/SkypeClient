namespace Skype.Client
{
    public class CallEventArgs
    {
        public CallAction Type { get; internal set; }

        public string CallerName { get; internal set; }

        public string CallId { get; internal set; }

        public override string ToString()
        {
            switch (Type)
            {
                case CallAction.Incoming: return $"Incoming call from '{CallerName}'. Call-Id: {CallId}";
                case CallAction.Accepted: return $"Call with '{CallerName}' started. Call-Id: {CallId}";
                case CallAction.Declined: return $"Declined call from '{CallerName}'. Call-Id: {CallId}";
                case CallAction.Missed: return $"Missed call from '{CallerName}'. Call-Id: {CallId}";
                case CallAction.Ended: return $"Call with '{CallerName}' ended. Call-Id: {CallId}";
            }

            return base.ToString();
        }
    }
}