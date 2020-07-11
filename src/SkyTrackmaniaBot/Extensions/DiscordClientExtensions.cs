using System;
using System.Linq;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Services;
using Validation;

namespace SkyTrackmaniaBot.Extensions
{
    public static class DiscordClientExtensions
    {
        public static IServiceCollection AddDiscordClient(this IServiceCollection collection,
            Action<DiscordOptions> options)
        {
            Requires.NotNull(collection, nameof(collection));
            
            collection.AddSingleton((services) => CreateModel(options));
            collection.AddSingleton<DiscordClient>();
            return collection;
        }

        public static IServiceCollection AddDiscordClientMessageHandlers(this IServiceCollection collection)
        {
            Requires.NotNull(collection, nameof(collection));
            Requires.That(collection.Any(x => x.ServiceType == typeof(IDiscordMessageSubscriberRegistry)), nameof(IDiscordMessageSubscriberRegistry), "IDiscordMessageSubscriberRegistry must be registered!");
            Requires.That(collection.Any(x => x.ServiceType == typeof(DiscordConfiguration)), nameof(DiscordConfiguration), "DiscordConfiguration must be registered!");
            Requires.That(collection.Any(x => x.ServiceType == typeof(DiscordClient)), nameof(DiscordClient), "DiscordClient must be registered!");

            collection.AddSingleton<IDiscordMessageHandler, DiscordMessageHandler>();
            return collection;
        }

        // WHAT THE FUCK...
        private static DiscordConfiguration CreateModel(Action<DiscordOptions> initialization)
        {
            var configuration = new DiscordConfiguration();
            var options = new DiscordOptions();
            initialization(options);

            configuration.Token = options.Token;
            return configuration;
        }
    }

    public class DiscordOptions
    {
        public string? Token { get; set; }
    }
}