using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Skype.Client.Channel;
using Skype.Client.Protocol.Contacts;
using Skype.Client.Protocol.Events;
using Skype.Client.Protocol.Events.Resource;
using Skype.Client.Protocol.Events.Resource.Content;
using Skype.Client.Protocol.General;
using Skype.Client.Protocol.People;
using Skype.Client.Protocol.Signaling.CallNotification;

namespace Skype.Client
{
    public class SkypeClient
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        protected MessageChannel CallSignalingChannel { get; }

        protected MessageChannel EventChannel { get; }

        protected MessageChannel PropertiesChannel { get; }

        protected MessageChannel ProfilesChannel { get; }

        protected MessageChannel ContactsChannel { get; }

        protected MessageChannel UserPresenceChannel { get; }

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

            CallSignalingChannel = new MessageChannel(nameof(CallSignalingChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));
            EventChannel = new MessageChannel(nameof(EventChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));
            PropertiesChannel = new MessageChannel(nameof(PropertiesChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));
            ProfilesChannel = new MessageChannel(nameof(ProfilesChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));
            ContactsChannel = new MessageChannel(nameof(ContactsChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));
            UserPresenceChannel = new MessageChannel(nameof(UserPresenceChannel), loggerFactory.CreateLogger(typeof(MessageChannel)));

            CallSignalingChannel.MessagePublished += CallSignalingChannelOnMessagePublished;
            EventChannel.MessagePublished += EventChannelOnMessagePublished;
            PropertiesChannel.MessagePublished += PropertiesChannelOnMessagePublished;
            ProfilesChannel.MessagePublished += ProfilesChannelOnMessagePublished;
            ContactsChannel.MessagePublished += ContactsChannelOnMessagePublished;
            UserPresenceChannel.MessagePublished += UserPresenceChannelOnMessagePublished;
        }

        private void ContactsChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var contactsFrame = JsonConvert.DeserializeObject<ContactsFrame>(e.Message);

            if (contactsFrame.Contacts != null)
            {
                foreach (var contact in contactsFrame.Contacts)
                {
                    if (!this.Contacts.Exists(p => p.Id == contact.Mri))
                    {
                        var contactDisplayName = !string.IsNullOrWhiteSpace(contact.DisplayName) ? contact.DisplayName : contact.Profile.Name.First;
                        var profile = new Profile(contact.Mri, contactDisplayName);

                        this.Contacts.Add(profile);
                        _logger.LogInformation("Found new contact: '{displayName}' ({id})", profile.DisplayName, profile.Id);
                    }
                }
            }
        }

        private void ProfilesChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var profileFrame = JsonConvert.DeserializeObject<ProfileFrame>(e.Message);

            if (profileFrame == null)
            {
                return;
            }

            foreach (var item in profileFrame.Profiles)
            {
                var profile = new Profile(item.Key, item.Value.Profile.DisplayName);
                    
                if (item.Value.Authorized)
                {
                    this.Me = profile;

                    if (this.Status != AppStatus.Ready)
                    {
                        _logger.LogInformation("Logged in as '{}', Id: {id}. Client is ready for interactions.", profile.DisplayName, profile.Id);
                        this.UpdateStatus(AppStatus.Ready);
                    }

                }
                else
                {
                    _logger.LogInformation("Found new contact: '{displayName}' ({id})", profile.DisplayName, profile.Id);
                    this.Contacts.Add(profile);
                }
            }
        }

        private void PropertiesChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            var props = JsonConvert.DeserializeObject<Properties>(e.Message);

            if (props != null)
            {
                this.Properties = props;
            }

            if (this.Status != AppStatus.Connected)
            {
                this.UpdateStatus(AppStatus.Connected);
            }
        }

        public Properties Properties { get; private set; }
        
        public Profile Me { get; set; }
        
        public List<Profile> Contacts { get; } = new List<Profile>();

        private void UserPresenceChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            
        }

        private void CallSignalingChannelOnMessagePublished(object sender, PublishMessageEventArgs e)
        {
            // Video or Audio Call Notification
            var notification = JsonConvert.DeserializeObject<CallNotificationFrame>(e.Message);

            if (notification?.Participants != null)
            {
                _logger.LogInformation("Incoming call from '{caller}'. Call-Id: {callId}", notification.Participants.From.DisplayName, notification.DebugContent.CallId);

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
            var messageFrame = JsonConvert.DeserializeObject<EventMessageFrame>(e.Message);
            
            if (messageFrame.EventMessages != null)
            {
                foreach (var eventMessage in messageFrame.EventMessages)
                {
                    if (HandleUserPresence(eventMessage)) continue;
                    
                    if (HandleEndpointPresence(eventMessage)) continue;
                    
                    if (HandleCallLogMessages(eventMessage)) continue;

                    if (HandleCallPreFlightEvent(eventMessage)) continue;

                    if (HandleChatMessage(eventMessage)) continue;

                    if (HandleTypingMessage(eventMessage)) continue;

                    if (HandleCallUpdates(eventMessage)) continue;

                    if (HandleCustomUserProperties(eventMessage)) continue;

                    OnUnhandledEventMessage(new EventMessageEventArgs {EventMessage = eventMessage});
                    _logger.LogWarning("Unable to handle eventMessage '{id}' of type '{type}' with resource type '{resourceType}'", eventMessage.Id, eventMessage.Type, eventMessage.ResourceType);
                }
            }
        }

        private bool HandleCustomUserProperties(EventMessage eventMessage)
        {
            return true;
        }

        private bool HandleCallPreFlightEvent(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is NewMessageResource res)) return false;

            if (res.MessageType == "Signal/Flamingo")
            {
                _logger.LogInformation("Incoming call from '{}'", res.CallerDisplayName);
                return true;
            }

            return false;
        }
        
        private bool HandleTypingMessage(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is NewMessageResource res)) return false;

            if (res.MessageType == "Control/Typing")
            {
                _logger.LogInformation("User '{}' is typing", res.ImDisplayName);
                return true;
            }

            return false;
        }

        private bool HandleEndpointPresence(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is EndpointPresenceResource res)) return false;

            return true;
        }

        private bool HandleUserPresence(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is UserPresenceResource res)) return false;

            return true;
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
            if (!(eventMessage.Resource is NewMessageResource res)) return false;

            var messageType = res.MessageType;
            if (messageType != "Event/Call")
            {
                return false;
            }

            var callInformationXmlString = res.Content;
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

            if (partsList.Type == "started")
            {
                _logger.LogInformation("Call with '{caller}' has started. Call-Id: {callId}", res.ImDisplayName, partsList.CallId);
            }
            else
            {
                _logger.LogInformation("Call with '{caller}' has ended. Call-Id: {callId}", res.ImDisplayName, partsList.CallId);
            }

            OnCallStatusChanged(new CallEventArgs
            {
                Type = partsList.Type == "started" ? CallAction.Accepted : CallAction.Ended,
                CallerName = res.ImDisplayName,
                CallId = partsList.CallId
            });

            return true;

        }

        private bool HandleChatMessage(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is NewMessageResource res))
            {
                return false;
            }

            var messageType = res.MessageType;
            if (messageType != "RichText")
            {
                return false;
            }

            var messageContent = res.Content;
            if (messageContent == null)
            {
                return false;
            }

            var senderProfileUrl = res.From;

            if (senderProfileUrl.Contains(this.Me.Id))
            {
                // Hide own own chat messages and return unhandled.
                return true;
            }

            _logger.LogInformation("Chat message received from '{sender}'. Content: {rawContent}", res.ImDisplayName, messageContent);

            OnMessageReceived(new MessageReceivedEventArgs
            {
                SenderName = res.ImDisplayName,
                MessageHtml = messageContent
            });

            return true;

        }

        private bool HandleCallLogMessages(EventMessage eventMessage)
        {
            if (!(eventMessage.Resource is NewMessageResource res))
            {
                return false;
            }

            var callLog = res?.Properties?.CallLog;
            if (callLog == null || callLog.CallState != "declined" && callLog.CallState != "missed")
            {
                return false;
            }

            if (callLog.CallState == "declined")
            {
                _logger.LogInformation("Declined call from '{caller}'. Call-Id: {callId}", callLog.OriginatorParticipant.DisplayName, callLog.CallId);
            }
            else
            {
                _logger.LogInformation("Missed call from '{caller}'. Call-Id: {callId}", callLog.OriginatorParticipant.DisplayName, callLog.CallId);
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
}