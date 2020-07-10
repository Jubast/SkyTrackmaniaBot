#nullable disable

namespace SkyTrackmaniaBot.Common.Models
{
    public class RecordInfo
    {
        public RecordInfo(RecordType recordType, string player, string time, string server, string mode, string replayUrl)
        {
            RecordType = recordType;
            Player = player;
            Time = time;
            Server = server;
            Mode = mode;
            ReplayUrl = replayUrl;
        }

        public RecordType RecordType { get; }
        public string Player { get; }
        public string Time { get; }
        public string Server { get; }
        public string Mode { get; }
        public string ReplayUrl { get; }

        public static RecordInfo CreateTMXRecordInfo(string player, string time, string replayUrl)
        {
            return new RecordInfo(RecordType.TrackmaniaExchange, player, time, "Offline", "Unknown", replayUrl);
        }
        
        public static RecordInfo CreateDediRecordInfo(string player, string time, string server, string mode)
        {
            return new RecordInfo(RecordType.Dedimania, player, time, server, mode, null);
        }
    }
}