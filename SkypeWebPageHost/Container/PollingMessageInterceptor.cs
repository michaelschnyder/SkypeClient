using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CefSharp;
using CefSharp.Extensions.Interception;
using Newtonsoft.Json;
using SkypeWebPageHost.Protocol.Events;
using SkypeWebPageHost.Protocol.Events.Resource.Content;

namespace SkypeWebPageHost.Container
{
    public class PollingMessageInterceptor : IRequestInterceptor
    {
        public void Execute(IResponse response, MemoryStream stream)
        {
            if (response.Charset != "utf-8" || stream.Length == 0) return;

            var str = Encoding.UTF8.GetString(stream.ToArray());
            var messageFrame = JsonConvert.DeserializeObject<Frame>(str);

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
    }
}