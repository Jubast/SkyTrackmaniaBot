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
    [RegexSubscription("https://tmnforever\\.tm-exchange\\.com/main\\.aspx\\?action=trackshow&id=(\\d+)")]
    public class TMNFInfoHandler : IDiscordMessageSubscriber
    {
        private readonly Match _match;
        private readonly ITMNFService _tmnfService;

        public TMNFInfoHandler(Match match, ITMNFService tmnfService)
        {
            Requires.NotNull(match, nameof(match));
            Requires.NotNull(tmnfService, nameof(tmnfService));

            _match = match;
            _tmnfService = tmnfService;
        }

        public async Task OnMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default)
        {
            if (!_match.Success)
                return;

            var tmxId = _match.Groups[1].Value;
            var trackInfo = await _tmnfService.GetTrackInformation(tmxId, cancellationToken);
            var embededMessage = DiscordEmbedHelper.CreateEmbedForTrackInfo(trackInfo);

            await messageCreated.Message.RespondAsync(null, false, embededMessage);
        }
    }
}