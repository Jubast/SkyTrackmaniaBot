using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace SkyTrackmaniaBot.Common.Interfaces
{
    public interface IDiscordMessageHandler
    {
        Task OnMessageCreated(MessageCreateEventArgs e);
        Task OnClientReady(ReadyEventArgs e);
        Task OnClientErrored(ClientErrorEventArgs e);
    }
}