#nullable disable

namespace SkyTrackmaniaBot.Common.Models
{
    public class MXTrackInfoResponse
    {
        public string TrackId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string StyleName { get; set; }
        public string LengthName { get; set; }
        public string TrackUID { get; set; }
        public int ReplayCount { get; set; }
    }
}