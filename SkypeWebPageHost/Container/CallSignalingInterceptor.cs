using System;
using System.IO;
using System.Text;
using CefSharp;
using Newtonsoft.Json;
using SkypeWebPageHost.CefSharpExtensions.Interception;
using SkypeWebPageHost.Protocol.Signaling.CallNotification;

namespace SkypeWebPageHost.Container
{
    public class CallSignalingInterceptor : IRequestInterceptor
    {
        public void Execute(IResponse response, MemoryStream stream)
        {
            if (response.Charset != "utf-8" || stream.Length == 0) return;

            var str = Encoding.UTF8.GetString(stream.ToArray());

            // Video or Audio Call Notification
            var notification = JsonConvert.DeserializeObject<CallNotificationFrame>(str);

            if (notification?.Participants != null)
            {
                Console.WriteLine($"Incoming from {notification.Participants.From.DisplayName}. Call-Id: {notification.DebugContent.CallId}");
            }
        }
    }
}