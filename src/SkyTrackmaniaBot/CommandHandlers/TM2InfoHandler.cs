using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using SkyTrackmaniaBot.Common.Attributes;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Common.Models;
using Validation;

namespace SkyTrackmaniaBot.CommandHandlers
{
    [RegexSubscription("https://tm\\.mania-exchange\\.com/tracks/(\\d+)")]
    public class TM2InfoHandler : IDiscordMessageSubscriber
    {
        private readonly Match _match;
        private readonly ITM2Service _tm2Service;
        private readonly ILogger _logger;

        public TM2InfoHandler(Match match, ITM2Service tm2Service, ILogger<TM2InfoHandler> logger)
        {
            Requires.NotNull(match, nameof(match));
            Requires.NotNull(tm2Service, nameof(tm2Service));
            Requires.NotNull(logger, nameof(logger));
            
            _match = match;
            _tm2Service = tm2Service;
            _logger = logger;
        }
        
        public async Task OnMessageCreated(MessageCreateEventArgs messageCreated, CancellationToken cancellationToken = default)
        {
            if (!_match.Success)
                return;

            // Remove auto embed for TM2 Url
            await messageCreated.Message.ModifyEmbedSuppressionAsync(true);
            
            var tmxId = _match.Groups[1].Value;
            var trackInfo = await _tm2Service.GetTrackInformation(tmxId, cancellationToken);
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