using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace SkyTrackmaniaBot.Common.Interfaces
{
    public interface IDiscordMessageSubscriber
    {
        Task OnMessageCreated(MessageCreateEventArgs messageCreated, CancellationToken cancellationToken = default);
    }
}