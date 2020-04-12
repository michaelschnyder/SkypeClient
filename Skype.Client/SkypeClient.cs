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
        public MessageChannel CallSignalingChannel { get; }
        public MessageChannel EventChannel { get; }

        public SkypeClient() : this(new MessageChannel(), new MessageChannel()) { }

        public SkypeClient(MessageChannel callSignalingChannel, MessageChannel eventChannel)
        {
            CallSignalingChannel = callSignalingChannel;
            EventChannel = eventChannel;

            callSignalingChannel.MessagePublished += CallSignalingChannelOnMessagePublished;
            eventChannel.MessagePublished += EventChannelOnMessagePublished;
        }

        private void EventChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var messageFrame = JsonConvert.DeserializeObject<Frame>(e.Message);

            var callLog = messageFrame.EventMessages?.Select(m => (m?.Resource?.Properties?.CallLog)).First();

            if (callLog != null)
            {
                if (callLog.CallState == "declined")
                {
                    Console.WriteLine($"Declined call from {callLog.OriginatorParticipant.DisplayName}. Call-Id: {callLog.CallId}"); return;
                }

                if (callLog.CallState == "missed")
                {
                    Console.WriteLine($"Missed call from {callLog.OriginatorParticipant.DisplayName}. Call-Id: {callLog.CallId}"); return;
                }
            }

            var messageContent = messageFrame.EventMessages?.Where(m => m.Resource?.MessageType == "RichText").Select(m => m.Resource.Content).FirstOrDefault();

            if (messageContent != null)
            {
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
                    Console.WriteLine($"Call with {messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName} started. Call-Id {partsList.CallId}");
                }
                if (partsList.Type == "ended")
                {
                    Console.WriteLine($"Call with {messageFrame.EventMessages.FirstOrDefault()?.Resource.ImDisplayName} ended. Call-Id {partsList.CallId}");
                }
            }
        }

        private void CallSignalingChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            // Video or Audio Call Notification
            var notification = JsonConvert.DeserializeObject<CallNotificationFrame>(e.Message);

            if (notification?.Participants != null)
            {
                Console.WriteLine($"Incoming from {notification.Participants.From.DisplayName}. Call-Id: {notification.DebugContent.CallId}");
            }


        }
    }
}