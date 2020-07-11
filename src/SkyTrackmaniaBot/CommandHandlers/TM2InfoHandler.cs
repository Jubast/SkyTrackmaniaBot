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
    [RegexSubscription("https://tm\\.mania-exchange\\.com/tracks/(\\d+)")]
    public class TM2InfoHandler : IDiscordMessageSubscriber
    {
        private readonly Match _match;
        private readonly ITM2Service _tm2Service;

        public TM2InfoHandler(Match match, ITM2Service tm2Service)
        {
            Requires.NotNull(match, nameof(match));
            Requires.NotNull(tm2Service, nameof(tm2Service));

            _match = match;
            _tm2Service = tm2Service;
        }

        public async Task OnMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default)
        {
            if (!_match.Success)
                return;

            var tmxId = _match.Groups[1].Value;
            var trackInfo = await _tm2Service.GetTrackInformation(tmxId, cancellationToken);
            var embededMessage = DiscordEmbedHelper.CreateEmbedForTrackInfo(trackInfo);

            await messageCreated.Message.RespondAsync(null, false, embededMessage);

            await Task.Delay(500, cancellationToken);
            
            // Remove auto embed for TM2020 Url
            await messageCreated.Message.ModifyEmbedSuppressionAsync(true);
        }
    }
}