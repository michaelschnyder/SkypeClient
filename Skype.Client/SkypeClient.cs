using System;
using System.IO;
using System.Linq;
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

        public SkypeClient()
        {
            CallSignalingChannel = new MessageChannel();
            EventChannel = new MessageChannel();

            CallSignalingChannel.MessagePublished += CallSignalingChannelOnMessagePublished;
            EventChannel.MessagePublished += EventChannelOnMessagePublished;
        }

        private void EventChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var messageFrame = JsonConvert.DeserializeObject<Frame>(e.Message);

            if (messageFrame.EventMessages != null)
            {
                foreach (var eventMessage in messageFrame.EventMessages)
                {
                    var callLog = eventMessage.Resource?.Properties?.CallLog;

                    if (callLog != null && (callLog.CallState == "declined" || callLog.CallState == "missed"))
                    {
                        OnCallStatusChanged(new CallEventArgs
                        {
                            Type = callLog.CallState == "declined" ? CallAction.Declined : CallAction.Missed,
                            CallerName = callLog.OriginatorParticipant.DisplayName,
                            CallId = callLog.CallId
                        });
                    }
                }
            }


            var messageContent = messageFrame.EventMessages?.Where(m => m.Resource?.MessageType == "RichText").Select(m => m.Resource.Content).FirstOrDefault();

            if (messageContent != null)
            {
                OnMessageReceived(new MessageReceivedEventArgs
                {
                    SenderName = messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName,
                    MessageHtml = messageFrame.EventMessages?.FirstOrDefault()?.Resource.Content
                });

                Console.WriteLine($"Message from {messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName}: {messageFrame.EventMessages?.FirstOrDefault()?.Resource.Content}");
            }


            var callStartedXml = messageFrame.EventMessages?.Where(m => m.Resource?.MessageType == "Event/Call").Select(m => m.Resource.Content).FirstOrDefault();

            if (callStartedXml != null)
            {
                var serializer = new XmlSerializer(typeof(ParticipantList));

                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(callStartedXml);
                MemoryStream callStartedXmlStream = new MemoryStream(byteArray);

                // convert stream to string
                var partsList = (ParticipantList)serializer.Deserialize(callStartedXmlStream);

                if (partsList.Type == "started")
                {
                    OnCallStatusChanged(new CallEventArgs
                    {
                        Type = CallAction.Accepted,
                        CallerName = messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName,
                        CallId = partsList.CallId
                    });
                }

                if (partsList.Type == "ended")
                {
                    OnCallStatusChanged(new CallEventArgs
                    {
                        Type = CallAction.Ended,
                        CallerName = messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName,
                        CallId = partsList.CallId
                    });
                }
            }
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
    }
}