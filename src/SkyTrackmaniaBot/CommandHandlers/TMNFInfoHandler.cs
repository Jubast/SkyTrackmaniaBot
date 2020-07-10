using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SkyTrackmaniaBot.Common.Attributes;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Common.Models;
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
        
        public async Task OnMessageCreated(MessageCreateEventArgs messageCreated, CancellationToken cancellationToken = default)
        {
            if (!_match.Success)
                return;

            var tmxId = _match.Groups[1].Value;
            var trackInfo = await _tmnfService.GetTrackInformation(tmxId, cancellationToken);
            var embededMessage = CreateEmbededMessage(trackInfo);

            await messageCreated.Message.RespondAsync(null, false, embededMessage);
        }
        
        private DiscordEmbed CreateEmbededMessage(TrackInfo trackInfo)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"Track name: {trackInfo.TrackName}"
            };
            embedBuilder.Description =
                $"Author: {trackInfo.TrackAuthor}\n" +
                $"Style: {trackInfo.TrackStyle}\n" +
                $"Track Length: {trackInfo.TrackLength}";

            CreateFieldWithRecords(embedBuilder, "Tmx Records:", trackInfo.RecordInfos.Where(x => x.RecordType == RecordType.TrackmaniaExchange));
            CreateFieldWithRecords(embedBuilder, "Dedi Records:", trackInfo.RecordInfos.Where(x => x.RecordType == RecordType.Dedimania));

            embedBuilder.ImageUrl = trackInfo.TrackImageUrl;
            embedBuilder.Color = DiscordColor.Azure; //DiscordColor.CornflowerBlue;
            return embedBuilder;
        }
        
        private void CreateFieldWithRecords(DiscordEmbedBuilder embed, string name, IEnumerable<RecordInfo> records)
        {
            StringBuilder builder = new StringBuilder();

            foreach (RecordInfo record in records.Where(x => x != null))
            {
                builder.Append(record.RecordType == RecordType.Dedimania
                    ? $"[{record.Time}] by {record.Player}\n"
                    : $"\\[[{record.Time}]({record.ReplayUrl})] by {record.Player} \n");
            }

            if (builder.Length == 0)
                return;
            
            embed.AddField(name, builder.ToString());
        }
    }
}