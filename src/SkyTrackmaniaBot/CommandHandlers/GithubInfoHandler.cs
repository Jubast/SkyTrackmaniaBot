using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SkyTrackmaniaBot.Common.Attributes;
using SkyTrackmaniaBot.Common.Interfaces;

namespace SkyTrackmaniaBot.CommandHandlers
{
    [CommandSubscription("!github")]
    public class GithubInfoHandler : IDiscordMessageSubscriber
    {
        public Task OnMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default)
        {
            var builder = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.SapGreen)
                .WithAuthor("Jubast")
                .WithThumbnail("https://avatars0.githubusercontent.com/u/30406814?s=460&v=4")
                .WithTitle("SkyTrackmaniaBot Repository")
                .WithUrl("https://github.com/Jubast/SkyTrackmaniaBot")
                .WithDescription("╔══════════════╗\n　　**(☞ ͡° ͜ʖ ͡° )☞**\n╚══════════════╝");

            return messageCreated.Message.RespondAsync(null, false, builder.Build());
        }
    }
}