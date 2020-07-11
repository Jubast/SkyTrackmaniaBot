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
    public class TM2020Service : ITM2020Service
    {
        private HttpClient _httpClient;

        public TM2020Service(HttpClient httpClient)
        {
            Requires.NotNull(httpClient, nameof(httpClient));
            _httpClient = httpClient;
        }
        
        public async Task<TrackInfo> GetTrackInformation(string tmxId, CancellationToken cancellationToken = default)
        {
            var info = await GetTrack(tmxId, cancellationToken);
            return CreateTrackInfo(tmxId, info);
        }

        private TrackInfo CreateTrackInfo(string tmxId, MXTrackInfoResponse info)
        {
            var trackInfo = new TrackInfo();
            trackInfo.TrackUrl = $"https://trackmania.exchange/tracks/view/{tmxId}";
            trackInfo.TrackImageUrl = $"https://trackmania.exchange/tracks/thumbnail/{tmxId}";
            trackInfo.TrackAuthor = info.Username;
            trackInfo.TrackName = info.Name;
            trackInfo.TrackLength = info.LengthName;
            trackInfo.TrackStyle = info.StyleName;
            
            return trackInfo;
        }

        private async Task<MXTrackInfoResponse> GetTrack(string tmxId, CancellationToken cancellationToken = default)
        {
            var trackInfoUrl = $"https://trackmania.exchange/api/tracks/get_track_info/multi/{tmxId}";
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