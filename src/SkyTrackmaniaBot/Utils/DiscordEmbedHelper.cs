using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;
using SkyTrackmaniaBot.Common.Models;

namespace SkyTrackmaniaBot.Utils
{
    public static class DiscordEmbedHelper
    {
        public static DiscordEmbed CreateEmbedForTrackInfo(TrackInfo trackInfo)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"Track name: {trackInfo.TrackName}",
                Url = trackInfo.TrackUrl
            };
            
            embedBuilder.Description =
                $"Author: {trackInfo.TrackAuthor}\n" +
                $"Style: {trackInfo.TrackStyle}\n" +
                $"Track Length: {trackInfo.TrackLength}";

            CreateFieldWithRecords(embedBuilder, "Tmx Records:", trackInfo.RecordInfos?.Where(x => x.RecordType == RecordType.TrackmaniaExchange));
            CreateFieldWithRecords(embedBuilder, "Dedi Records:", trackInfo.RecordInfos?.Where(x => x.RecordType == RecordType.Dedimania));

            embedBuilder.ImageUrl = trackInfo.TrackImageUrl;
            embedBuilder.Color = DiscordColor.CornflowerBlue;
            return embedBuilder;
        }
        
        private static void CreateFieldWithRecords(DiscordEmbedBuilder embed, string name, IEnumerable<RecordInfo>? records)
        {
            if (records == null)
                return;
            
            var builder = new StringBuilder();
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