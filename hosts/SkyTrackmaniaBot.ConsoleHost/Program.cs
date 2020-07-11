using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SkyTrackmaniaBot.CommandHandlers;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Extensions;

namespace SkyTrackmaniaBot.ConsoleHost
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.json")
                .AddJsonFile("Config.local.json")
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog(logger))
                .AddDiscordClient(config.GetSection("DSharpPlus").Bind!)
                .AddDiscordClientMessageHandlers()
                .AddTrackmaniaServices()
                .BuildServiceProvider();

            var registry = services.GetService<IDiscordMessageSubscriberRegistry>();
            registry.AddSubscriber<TMNFInfoHandler>();
            registry.AddSubscriber<TM2InfoHandler>();
            
            var client = services.GetService<DiscordClient>();
            var messageHandler = services.GetService<IDiscordMessageHandler>();

            client.Ready += messageHandler.OnClientReady;
            client.MessageCreated += messageHandler.OnMessageCreated;
            client.ClientErrored += messageHandler.OnClientErrored;

            await client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}