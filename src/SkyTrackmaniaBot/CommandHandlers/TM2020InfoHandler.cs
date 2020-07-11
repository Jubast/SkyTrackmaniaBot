using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using SkyTrackmaniaBot.Common.Attributes;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Utils;
using Validation;

namespace SkyTrackmaniaBot.CommandHandlers
{
    [RegexSubscription("https://trackmania\\.exchange/tracks/view/(\\d+)")]
    public class TM2020InfoHandler : IDiscordMessageSubscriber
    {
        private readonly Match _match;
        private readonly ITM2020Service _tm2020Service;

        public TM2020InfoHandler(Match match, ITM2020Service tm2020Service)
        {
            Requires.NotNull(match, nameof(match));
            Requires.NotNull(tm2020Service, nameof(tm2020Service));

            _match = match;
            _tm2020Service = tm2020Service;
        }

        public async Task OnMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default)
        {
            if (!_match.Success)
                return;

            var tmxId = _match.Groups[1].Value;
            var trackInfo = await _tm2020Service.GetTrackInformation(tmxId, cancellationToken);
            var embededMessage = DiscordEmbedHelper.CreateEmbedForTrackInfo(trackInfo);

            await messageCreated.Message.RespondAsync(null, false, embededMessage);
            
            await Task.Delay(500, cancellationToken);
            
            // Remove auto embed for TM2020 Url
            await messageCreated.Message.ModifyEmbedSuppressionAsync(true);
        }
    }
}