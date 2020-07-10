using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace SkyTrackmaniaBot.Common.Interfaces
{
    public interface IDiscordMessageSubscriberRegistry
    {
        void AddSubscriber<T>() where T : class, IDiscordMessageSubscriber;

        Task PublishMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default);
    }
}