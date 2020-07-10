using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Common.Models;
using Validation;

namespace SkyTrackmaniaBot.Services
{
    public class TMNFService : ITMNFService
    {
        private HttpClient _httpClient;

        public TMNFService(HttpClient httpClient)
        {
            Requires.NotNull(httpClient, nameof(httpClient));
            _httpClient = httpClient;
        }

        public async Task<TrackInfo> GetTrackInformation(string tmxId, CancellationToken cancellationToken = default)
        {
            var tmxHtml = await GetTMXHtml(tmxId, cancellationToken);
            var dediHtml = await GetDediHtml(GetTrackUid(tmxHtml), cancellationToken);
            return CreateTrackInfo(tmxId, tmxHtml, dediHtml);
        }
        
        private TrackInfo CreateTrackInfo(string tmxId, string tmxHtml, string dediHtml)
        {
            TrackInfo t = new TrackInfo();
            t.TrackUrl = $"https://tmnforever.tm-exchange.com/main.aspx?action=trackshow&id={tmxId}";;
            t.TrackImageUrl = $"https://tmnforever.tm-exchange.com/getclean.aspx?action=trackscreenscreens&id={tmxId}&screentype=1";

            var match = Regex.Match(tmxHtml, "target=\"_blank\">(.*?)</a></TD>");
            t.TrackAuthor = HtmlDecodeOrEmpty(match);

            match = Regex.Match(tmxHtml, "id=\"ctl03_ShowTrackName\">(.*?)</span>");
            t.TrackName = HtmlDecodeOrEmpty(match);

            match = Regex.Match(tmxHtml, "id=\"ctl03_ShowLength\">(.*?)</span>");
            t.TrackLength = HtmlDecodeOrEmpty(match);

            match = Regex.Match(tmxHtml, "id=\"ctl03_ShowStyle\">(.*?)</span>");
            t.TrackStyle = HtmlDecodeOrEmpty(match);

            t.RecordInfos = CreateRecordInfos(tmxHtml, dediHtml);
            return t;
        }

        private List<RecordInfo> CreateRecordInfos(string tmxHtml, string dediHtml)
        {
            var recordInfos = new List<RecordInfo>(6);
            recordInfos.AddRange(CreateTmxRecordInfos(tmxHtml));
            recordInfos.AddRange(CreateDediRecordInfos(dediHtml));
            return recordInfos;
        }

        private List<RecordInfo> CreateTmxRecordInfos(string tmxHtml)
        {
            var recordInfos = new List<RecordInfo>(3);
            var replayCollection = Regex.Matches(tmxHtml, "get\\.aspx\\?action=recordgbx&amp;id=(.*?)\"");
            foreach (Match m in replayCollection)
            {
                if (!m.Success)
                    continue;
                
                if(recordInfos.Count >= 3)
                    break;

                var time = GetTmxTime(m, tmxHtml);
                string replayUrl = null!;
                var replayId = HtmlDecodeOrEmpty(m); 
                if (replayId == string.Empty)
                    replayUrl = "https://image.slidesharecdn.com/memes-150616230659-lva1-app6892/95/parody-" +
                                "sharepoint-memes-for-modern-dialogs-15-638.jpg?cb=1434496813";
                else
                    replayUrl = $"https://tmnforever.tm-exchange.com/get.aspx?action=recordgbx&id={replayId}";
                
                var match = Regex.Match(tmxHtml.Substring(m.Index), "target=\"_blank\">(.*?)</a></td><td>");
                var player = HtmlDecodeOrEmpty(match);

                var recordInfo = RecordInfo.CreateTMXRecordInfo(player, time, replayUrl);
                recordInfos.Add(recordInfo);
            }

            return recordInfos;
        }
        
        private List<RecordInfo> CreateDediRecordInfos(string dediHtml)
        {
            var recordInfos = new List<RecordInfo>(3);
            MatchCollection collection = Regex.Matches(dediHtml, "<td>(.*?)</td>");
            for (int i = 0; i < collection.Count; i += 4)
            {
                if(recordInfos.Count >= 3)
                    break;
                
                var player = HtmlDecodeOrEmpty(collection[i]);
                var server = HtmlDecodeOrEmpty(collection[i + 1]);
                var time = HtmlDecodeOrEmpty(collection[i + 2]);
                var mode = HtmlDecodeOrEmpty(collection[i + 3]);

                var recordInfo = RecordInfo.CreateDediRecordInfo(player, time, server, mode);
                recordInfos.Add(recordInfo);
            }

            return recordInfos;
        }
        
        private string GetTmxTime(Match m, string tmxHtml)
        {
            List<char> array= new List<char>();
            int index = m.Index + m.ToString().Length + 40;
            while (true)
            {
                char c = tmxHtml[index++];
                if (c != '\r')
                {
                    array.Add(c);
                }
                else
                {
                    break;
                }
            }
            
            return new string(array.ToArray());
        }

        private async Task<string> GetTMXHtml(string tmxId, CancellationToken cancellationToken = default)
        {
            var trackUrl = $"https://tmnforever.tm-exchange.com/main.aspx?action=trackshow&id={tmxId}";
            using var response = await _httpClient.GetAsync(trackUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
        private async Task<string> GetDediHtml(string trackUid, CancellationToken cancellationToken = default)
        {
            var dediRecordsUrl = $"https://tmnforever.tm-exchange.com/get.aspx?action=apidedimania&method=onlinerecords&uid={trackUid}";
            using var response = await _httpClient.GetAsync(dediRecordsUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private string GetTrackUid(string tmxHtml)
        {
            var m = Regex.Match(tmxHtml, "id=\"ctl03_TrackUid\" value=\"(.*?)\"");
            return HtmlDecodeOrEmpty(m);
        }

        private string HtmlDecodeOrEmpty(Match match)
        {
            if (match.Success && match.Groups.Count >= 1)
            {
                return WebUtility.HtmlDecode(match.Groups[1].Value);
            }

            return string.Empty;
        }
    }
}