using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skype.Client.Channel;


namespace Skype.Client.Tests
{
    public class TestableClient : SkypeClient
    {
        public TestableClient(ILoggerFactory loggerFactory): base(loggerFactory)
        {
        }

        internal new MessageChannel CallSignalingChannel => base.CallSignalingChannel;

        internal new MessageChannel EventChannel => base.EventChannel;

        internal new MessageChannel PropertiesChannel => base.PropertiesChannel;

        internal new MessageChannel ProfilesChannel => base.ProfilesChannel;
    }

    [TestClass]
    public class MessageHandlingTest
    {
        private readonly TestableClient _sut;

        public MessageHandlingTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });

            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            services.AddSingleton<TestableClient>();
            var serviceProvider = services.BuildServiceProvider();

            _sut = serviceProvider.GetService<TestableClient>();
        }

        [TestMethod]
        public void Receives_EmptyJsonObjectString_NoCrash()
        {
            _sut.EventChannel.PublishMessage("{}");
        }

        [TestMethod]
        public void Receives_Frame_InboundMessage_TriggersEvent()
        {
            _sut.ProfilesChannel.PublishMessage(File.ReadAllText(@"profile-frame-self.json"));
            
            MessageReceivedEventArgs messageReceived = null;
            _sut.MessageReceived += (sender, args) => messageReceived = args;
            _sut.EventChannel.PublishMessage(File.ReadAllText(@"event-frame-with-incoming-message.json"));

            Assert.IsNotNull(messageReceived);
            Assert.AreEqual("Hello You :)", messageReceived.MessageHtml);
        }

        [TestMethod]
        public void Receives_Frame_OutboundMessage_NoEvent()
        {
            _sut.ProfilesChannel.PublishMessage(File.ReadAllText(@"profile-frame-self.json"));
            
            MessageReceivedEventArgs messageReceived = null;
            _sut.MessageReceived += (sender, args) => messageReceived = args;
            _sut.EventChannel.PublishMessage(File.ReadAllText(@"event-frame-with-outgoing-message.json"));

            Assert.IsNull(messageReceived);
        }

        [TestMethod]
        public void Receives_Properties_SavesThem()
        {
            _sut.PropertiesChannel.PublishMessage(File.ReadAllText(@"properties-message.json"));

            Assert.IsNotNull(_sut.Properties);
            Assert.AreEqual("live:.cid.e7f3f00ae2aa6068", _sut.Properties.primaryMemberName);
        }

        [TestMethod]
        public void Receives_OwnProfile_SavesIt()
        {
            _sut.ProfilesChannel.PublishMessage(File.ReadAllText(@"profile-frame-self.json"));

            Assert.IsNotNull(_sut.Me);
            Assert.AreEqual("8:live:.cid.e7f3f00ae2aa6068", _sut.Me.Id);
            Assert.AreEqual("Michael (Headless Account)", _sut.Me.DisplayName);
        }

        [TestMethod]
        public void Receives_OtherProfiles_AddContact()
        {
            _sut.ProfilesChannel.PublishMessage(File.ReadAllText(@"profile-frame-list.json"));

            Assert.IsNull(_sut.Me);

            Assert.AreEqual(1, _sut.Contacts.Count);

        }

    }
}
