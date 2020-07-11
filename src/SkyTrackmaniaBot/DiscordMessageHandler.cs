using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using SkyTrackmaniaBot.Common.Interfaces;
using Validation;

namespace SkyTrackmaniaBot
{
    public class DiscordMessageHandler : IDiscordMessageHandler
    {
        private readonly ILogger _logger;
        private readonly IDiscordMessageSubscriberRegistry _subscriberRegistry;

        public DiscordMessageHandler(ILogger<DiscordMessageHandler> logger, IDiscordMessageSubscriberRegistry subscriberRegistry)
        {
            Requires.NotNull(logger, nameof(logger));
            Requires.NotNull(subscriberRegistry, nameof(subscriberRegistry));
            
            _logger = logger;
            _subscriberRegistry = subscriberRegistry;
        }

        public Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return Task.CompletedTask;
            
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            return _subscriberRegistry.PublishMessageCreated(e, cts.Token);
        }

        public Task OnClientReady(ReadyEventArgs e)
        {
            _logger.LogInformation("Discord Client is ready");
            return Task.CompletedTask;
        }

        public Task OnClientErrored(ClientErrorEventArgs e)
        {
            _logger.LogError(e.Exception, $"Discord client received an unhandled error {e.EventName}.");
            return Task.CompletedTask;
        }
    }
}