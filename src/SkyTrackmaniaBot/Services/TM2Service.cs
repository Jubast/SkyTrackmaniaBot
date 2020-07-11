using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Common.Models;
using Validation;

namespace SkyTrackmaniaBot.Services
{
    public class TM2Service : ITM2Service
    {
        private HttpClient _httpClient;

        public TM2Service(HttpClient httpClient)
        {
            Requires.NotNull(httpClient, nameof(httpClient));
            _httpClient = httpClient;
        }
        
        public async Task<TrackInfo> GetTrackInformation(string tmxId, CancellationToken cancellationToken = default)
        {
            var info = await GetTrack(tmxId, cancellationToken);
            var dediHtml = await GetDediHtml(info.TrackUID, cancellationToken);
            var replays = new List<MXReplayResponse>();
            if (info.ReplayCount > 0)
                replays = await GetReplays(tmxId, cancellationToken);

            return CreateTrackInfo(tmxId, info, replays, dediHtml);
        }

        private TrackInfo CreateTrackInfo(string tmxId, MXTrackInfoResponse info, List<MXReplayResponse> records, string dediHtml)
        {
            var trackInfo = new TrackInfo();
            trackInfo.TrackUrl = $"https://tm.mania-exchange.com/tracks/{tmxId}";
            trackInfo.TrackImageUrl = $"https://tm.mania-exchange.com/tracks/screenshot/normal/{tmxId}";
            trackInfo.TrackAuthor = info.Username;
            trackInfo.TrackName = info.Name;
            trackInfo.TrackLength = info.LengthName;
            trackInfo.TrackStyle = info.StyleName;
            trackInfo.RecordInfos = CreateRecordInfos(records, dediHtml);
            
            return trackInfo;
        }
        
        private List<RecordInfo> CreateRecordInfos(List<MXReplayResponse> records, string dediHtml)
        {
            var recordInfos = new List<RecordInfo>(6);
            recordInfos.AddRange(CreateTmxRecordInfos(records));
            recordInfos.AddRange(CreateDediRecordInfos(dediHtml));
            return recordInfos;
        }

        private List<RecordInfo> CreateTmxRecordInfos(List<MXReplayResponse> tmxReplays)
        {
            var recordInfos = new List<RecordInfo>(3);
            foreach (var replay in tmxReplays)
            {
                if(recordInfos.Count >= 3)
                    break;
                
                var replayUrl = $"https://tm.mania-exchange.com/replays/download/{replay.ReplayId}";
                var time = TimeSpan.FromMilliseconds(replay.ReplayTime).ToString("mm\\:ss\\.fff");
                var record = RecordInfo.CreateTMXRecordInfo(replay.Username, time, replayUrl);
                recordInfos.Add(record);
            }

            return recordInfos;
        }

        private List<RecordInfo> CreateDediRecordInfos(string dediHtml)
        {
            var recordInfos = new List<RecordInfo>(3);
            MatchCollection collection = Regex.Matches(dediHtml, "<td.*?>(.*?)</td>");
            for (int i = 4; i < collection.Count; i += 4)
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

        private async Task<string> GetDediHtml(string trackUid, CancellationToken cancellationToken = default)
        {
            var dediRecordsUrl = $"https://tm.mania-exchange.com/api/dedimania/get_online_records/{trackUid}";
            using var response = await _httpClient.GetAsync(dediRecordsUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
        private async Task<List<MXReplayResponse>> GetReplays(string tmxId, CancellationToken cancellationToken = default)
        {
            var replaysUrl = $"https://api.mania-exchange.com/tm/replays/{tmxId}/3";
            using var response = await _httpClient.GetAsync(replaysUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MXReplayResponse>>(json);
        }

        private async Task<MXTrackInfoResponse> GetTrack(string tmxId, CancellationToken cancellationToken = default)
        {
            var trackInfoUrl = $"https://api.mania.exchange/tm/maps/{tmxId}";
            using var response = await _httpClient.GetAsync(trackInfoUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MXTrackInfoResponse>>(json).FirstOrDefault();
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