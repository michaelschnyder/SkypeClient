using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CefSharp;
using CefSharp.Handler;
using Newtonsoft.Json;
using SkypeWebPageHost.Protocol.Events;
using SkypeWebPageHost.Protocol.Events.Resource.Content;
using SkypeWebPageHost.Protocol.Signaling.CallNotification;

namespace SkypeWebPageHost.Container
{
    public class RequestHandlerInterceptionFactory : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            if (request.Url.Contains("cc.skype.com/cc/v1")) 
            { 
                return new CustomResourceRequestHandler((response, stream) =>
                {
                    if (response.Charset != "utf-8" || stream.Length == 0) return;

                    //You can now get the data from the stream
                    var bytes = stream.ToArray();
                    var str = Encoding.UTF8.GetString(bytes);

                    // Video or Audio Call Notification
                    var notification = JsonConvert.DeserializeObject<CallNotificationFrame>(str);

                    if (notification?.Participants != null)
                    {
                        Console.WriteLine($"Incoming from {notification.Participants.From.DisplayName}. Call-Id: {notification.DebugContent.CallId}");
                    }

                });
            }

            if (request.Url.Contains("gateway.messenger.live.com/v1"))
            {
                return new CustomResourceRequestHandler((response, stream) =>
                {
                    if (response.Charset != "utf-8" || stream.Length == 0) return;

                    //You can now get the data from the stream
                    var bytes = stream.ToArray();
                    var str = Encoding.UTF8.GetString(bytes);

                    // Video or Audio Call Notification
                    var messageFrame = JsonConvert.DeserializeObject<Frame>(str);

                    var callLog = messageFrame.EventMessages?.Select(m => (m?.Resource?.Properties?.CallLog)).First();

                    if (callLog != null)
                    {
                        if (callLog.CallState == "declined")
                        {
                            Console.WriteLine(
                                $"Declined call from {callLog.OriginatorParticipant.DisplayName}. Call-Id: {callLog.CallId}");
                            return;
                        }

                        if (callLog.CallState == "missed")
                        {
                            Console.WriteLine(
                                $"Missed call from {callLog.OriginatorParticipant.DisplayName}. Call-Id: {callLog.CallId}");
                            return;
                        }
                    }

                    var messageContent = messageFrame.EventMessages?.Where(m => m.Resource?.MessageType == "RichText")
                        .Select(m => m.Resource.Content).FirstOrDefault();

                    if (messageContent != null)
                    {
                        Console.WriteLine($"Message from {messageFrame.EventMessages?.FirstOrDefault()?.Resource.ImDisplayName}: {messageFrame.EventMessages?.FirstOrDefault()?.Resource.Content}");
                    }


                    var callStartedXml = messageFrame.EventMessages
                        ?.Where(m => m.Resource?.MessageType == "Event/Call").Select(m => m.Resource.Content)
                        .FirstOrDefault();

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
                });
            }

            return null;
        }
    }
}