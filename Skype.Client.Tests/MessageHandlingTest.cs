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
    }

    [TestClass]
    public class MessageHandlingTest
    {
        private TestableClient sut;

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

            sut = serviceProvider.GetService<TestableClient>();
        }

        [TestMethod]
        public void Receives_EmptyJsonObjectString_NoCrash()
        {
            sut.EventChannel.PublishMessage("{}");
        }
    }
}
