using System.Collections.Generic;

#nullable disable

namespace SkyTrackmaniaBot.Common.Models
{
    public class TrackInfo
    {
        public string TrackName { get; set; }
        public string TrackAuthor { get; set; }
        public string TrackStyle { get; set; }
        public string TrackLength { get; set; }
        public string TrackUrl { get; set; }
        public string TrackImageUrl { get; set; }
        public List<RecordInfo> RecordInfos { get; set; }
    }
}