using DSharpPlus.EventArgs;
using SkyTrackmaniaBot.CommandHandlers;
using Xunit;
using Xunit.Abstractions;

namespace SkyTrackmaniaBot.Tests
{
    public class TrackmaniaCommandHandlerTests : BaseTestKit
    {
        public TrackmaniaCommandHandlerTests(ITestOutputHelper helper) : base(helper)
        {
        }
        
        [Fact]
        public void OnMessage_should_succeed()
        {
            Registry.AddSubscriber<TMNFInfoHandler>();
        }
    }
}