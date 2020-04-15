using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Skype.Client.Channel;
using Skype.Client.Protocol.Events;
using Skype.Client.Protocol.Events.Resource.Content;
using Skype.Client.Protocol.Signaling.CallNotification;

namespace Skype.Client
{
    public class SkypeClient
    {
        private readonly ILoggerFactory _loggerFactory;
        private ILogger _logger;

        protected MessageChannel CallSignalingChannel { get; }
        protected MessageChannel EventChannel { get; }

        public event EventHandler<CallEventArgs> IncomingCall;
        public event EventHandler<CallEventArgs> CallStatusChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived; 
        public event EventHandler<EventMessageEventArgs> UnhandledEventMessage;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public AppStatus Status { get; private set; }

        public SkypeClient() : this(NullLoggerFactory.Instance)
        {
        }

        public SkypeClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(SkypeClient));
            _loggerFactory = loggerFactory;

            CallSignalingChannel = new MessageChannel(loggerFactory.CreateLogger(typeof(MessageChannel)));
            EventChannel = new MessageChannel(loggerFactory.CreateLogger(typeof(MessageChannel)));

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
            if (this.Status != AppStatus.Connected)
            {
                this.UpdateStatus(AppStatus.Connected);
            }
            
            var messageFrame = JsonConvert.DeserializeObject<Frame>(e.Message);
            
            if (messageFrame.EventMessages != null)
            {
                foreach (var eventMessage in messageFrame.EventMessages)
                {
                    if (HandleCallLogMessages(eventMessage)) continue;

                    if (HandleChatMessage(eventMessage)) continue;

                    if (HandleCallUpdates(eventMessage)) continue;

                    OnUnhandledEventMessage(new EventMessageEventArgs {EventMessage = eventMessage});
                    _logger.LogWarning("Unable to handle eventMessage '{id}' {eventMessage}", eventMessage.Id,
                        JsonConvert.SerializeObject(eventMessage));
                }
            }

            if (messageFrame.SyncMessages != null)
            {
                foreach (var syncMessage in messageFrame.SyncMessages)
                {
                    _logger.LogWarning("Unable to handle event '{id}' {eventMessage}", syncMessage.Id, JsonConvert.SerializeObject(syncMessage));
                }
            }

            if (messageFrame.Responses != null)
            {
                foreach (var response in messageFrame.Responses)
                {
                    _logger.LogWarning("Unable to handle response {eventMessage}", JsonConvert.SerializeObject(response));
                }
            }

            if (messageFrame.EventMessages == null && messageFrame.SyncMessages == null && messageFrame.Responses == null)
            {
                _logger.LogWarning("Could not understand message frame {rawMessage}", e.Message);
            }
        }

        protected void UpdateStatus(AppStatus appStatus)
        {
            if (this.Status == appStatus)
            {
                return;
            }
            
            var oldStatus = appStatus;
            this.Status = appStatus;

            OnStatusChanged(new StatusChangedEventArgs {Old = oldStatus, New = appStatus } );
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

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }
    }

    public class StatusChangedEventArgs
    {
        public AppStatus Old { get; set; }

        public AppStatus New { get; set; }
    }

    public enum AppStatus
    {
        None,
        Connected,
        Initializing,
        Authenticating,
        Authenticated,
        Loading
    }
}