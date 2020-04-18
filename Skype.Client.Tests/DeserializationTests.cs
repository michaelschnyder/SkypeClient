using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Skype.Client.Protocol.Events;
using Skype.Client.Protocol.Events.Resource;

namespace Skype.Client.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void EventMessage_ResourceType_NewMessage()
        {

            var json = "{ " +
                       "   \"ResourceType\": \"NewMessage\"," +
                       "   \"Resource\": {" +
                       "      \"Content\": \"Hello!\"" +
                       "    }" +
                       "}";

            var eventMessage = JsonConvert.DeserializeObject<EventMessage>(json);

            Assert.IsNotNull(eventMessage.Resource);
            Assert.AreEqual(typeof(NewMessageResource), eventMessage.Resource.GetType());
            Assert.AreEqual("Hello!", ((NewMessageResource)eventMessage.Resource).Content);
        }

        [TestMethod]
        public void EventMessage_ResourceType_UserPresence()
        {

            var json = "{ " +
                       "   \"ResourceType\": \"UserPresence\"," +
                       "   \"Resource\": {" +
                       "      \"Status\": \"busy\"" +
                       "    }" +
                       "}";

            var eventMessage = JsonConvert.DeserializeObject<EventMessage>(json);

            Assert.IsNotNull(eventMessage.Resource);
            Assert.AreEqual(typeof(UserPresenceResource), eventMessage.Resource.GetType());
            Assert.AreEqual("busy", ((UserPresenceResource)eventMessage.Resource).Status);
        }

        [TestMethod]
        public void EventMessage_ResourceType_EndpointPresence()
        {

            var json = "{ " +
                       "   \"ResourceType\": \"EndpointPresence\"," +
                       "   \"Resource\": {" +
                       "      \"selflink\": \"http://t.ld\"" +
                       "    }" +
                       "}";

            var eventMessage = JsonConvert.DeserializeObject<EventMessage>(json);

            Assert.IsNotNull(eventMessage.Resource);
            Assert.AreEqual(typeof(EndpointPresenceResource), eventMessage.Resource.GetType());
            Assert.AreEqual("http://t.ld", ((EndpointPresenceResource)eventMessage.Resource).SelfLink);

        }
    }
}
