using System;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Extensions.Logging;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Services;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SkyTrackmaniaBot.Tests
{
    public abstract class BaseTestKit
    {
        protected ILogger<DiscordMessageHandler> Logger { get; private set; }
        protected IDiscordMessageHandler Handler { get; private set; }
        protected IDiscordMessageSubscriberRegistry Registry { get; private set; }
        protected Mock<IServiceProvider> ServiceProviderMock { get; private set; }

        protected BaseTestKit(ITestOutputHelper helper)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(helper)
                .CreateLogger();

            Logger = LoggerFactory.Create(b => b.AddSerilog(logger)).CreateLogger<DiscordMessageHandler>();

            ServiceProviderMock = new Mock<IServiceProvider>();
            ServiceProviderMock.Setup(x => x.GetService(typeof(BaseTestKit))).Returns(() => default!);

            Registry = new DiscordMessageSubscriberRegistry(ServiceProviderMock.Object);
            Handler = new DiscordMessageHandler(Logger, Registry);
        }
    }
}