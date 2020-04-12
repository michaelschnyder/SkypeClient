using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Skype.Client.Channel;
using Skype.Client.Protocol.Events;
using Skype.Client.Protocol.Events.Resource.Content;
using Skype.Client.Protocol.Signaling.CallNotification;

namespace Skype.Client
{
    public class SkypeClient
    {
        protected MessageChannel CallSignalingChannel { get; }
        protected MessageChannel EventChannel { get; }

        public event EventHandler<CallEventArgs> IncomingCall;
        public event EventHandler<CallEventArgs> CallStatusChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived; 
        public event EventHandler<EventMessageEventArgs> UnhandledEventMessage; 

        public SkypeClient()
        {
            CallSignalingChannel = new MessageChannel();
            EventChannel = new MessageChannel();

            CallSignalingChannel.MessagePublished += CallSignalingChannelOnMessagePublished;
            EventChannel.MessagePublished += EventChannelOnMessagePublished;
        }

        private void CallSignalingChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            // Video or Audio Call Notification
            var notification = JsonConvert.DeserializeObject<CallNotificationFrame>(e.Message);

            if (notification?.Participants != null)
            {
                OnIncomingCall(new CallEventArgs
                {
                    Type = CallAction.Incoming,
                    CallerName = notification.Participants.From.DisplayName,
                    CallId = notification.DebugContent.CallId
                });
            }
        }

        private void EventChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var messageFrame = JsonConvert.DeserializeObject<Frame>(e.Message);
            if (messageFrame.EventMessages == null) return;

            foreach (var eventMessage in messageFrame.EventMessages)
            {
                if (HandleCallLogMessages(eventMessage)) continue;

                if (HandleChatMessage(eventMessage)) continue;

                if (HandleCallUpdates(eventMessage)) continue;

                OnUnhandledEventMessage(new EventMessageEventArgs {EventMessage = eventMessage});
            }
        }

        private bool HandleCallUpdates(EventMessage eventMessage)
        {
            var messageType = eventMessage.Resource?.MessageType;
            if (messageType != "Event/Call")
            {
                return false;
            }

            var callInformationXmlString = eventMessage.Resource.Content;
            if (callInformationXmlString == null)
            {
                return false;
            }

            var serializer = new XmlSerializer(typeof(ParticipantList));
            var byteArray = Encoding.UTF8.GetBytes(callInformationXmlString);
            var callStartedXmlStream = new MemoryStream(byteArray);
            var partsList = (ParticipantList) serializer.Deserialize(callStartedXmlStream);
            if (partsList == null || (partsList.Type != "started" && partsList.Type != "ended"))
            {
                return false;
            }

            OnCallStatusChanged(new CallEventArgs
            {
                Type = partsList.Type == "started" ? CallAction.Accepted : CallAction.Ended,
                CallerName = eventMessage.Resource.ImDisplayName,
                CallId = partsList.CallId
            });

            return true;

        }

        private bool HandleChatMessage(EventMessage eventMessage)
        {
            var messageType = eventMessage.Resource?.MessageType;
            if (messageType != "RichText")
            {
                return false;
            }

            var messageContent = eventMessage.Resource.Content;
            if (messageContent == null)
            {
                return false;
            }

            OnMessageReceived(new MessageReceivedEventArgs
            {
                SenderName = eventMessage.Resource.ImDisplayName,
                MessageHtml = messageContent
            });

            return true;
        }

        private bool HandleCallLogMessages(EventMessage eventMessage)
        {
            var callLog = eventMessage.Resource?.Properties?.CallLog;
            if (callLog == null || callLog.CallState != "declined" && callLog.CallState != "missed")
            {
                return false;
            }

            OnCallStatusChanged(new CallEventArgs
            {
                Type = callLog.CallState == "declined" ? CallAction.Declined : CallAction.Missed,
                CallerName = callLog.OriginatorParticipant.DisplayName,
                CallId = callLog.CallId
            });

            return true;
        }

        protected virtual void OnIncomingCall(CallEventArgs e)
        {
            IncomingCall?.Invoke(this, e);
        }

        protected virtual void OnCallStatusChanged(CallEventArgs e)
        {
            CallStatusChanged?.Invoke(this, e);
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnUnhandledEventMessage(EventMessageEventArgs e)
        {
            UnhandledEventMessage?.Invoke(this, e);
        }
    }
}