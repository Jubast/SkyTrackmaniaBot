using System.Threading;
using System.Threading.Tasks;
using SkyTrackmaniaBot.Common.Models;

namespace SkyTrackmaniaBot.Common.Interfaces
{
    public interface ITM2Service
    {
        Task<TrackInfo> GetTrackInformation(string tmxId, CancellationToken cancellationToken = default);
    }
}