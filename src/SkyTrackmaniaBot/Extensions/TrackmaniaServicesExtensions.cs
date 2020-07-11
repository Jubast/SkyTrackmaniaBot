using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SkyTrackmaniaBot.CommandHandlers;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.Services;
using Validation;

namespace SkyTrackmaniaBot.Extensions
{
    public static class TrackmaniaServicesExtensions
    {
        public static IServiceCollection AddTrackmaniaServices(this IServiceCollection collection)
        {
            Requires.NotNull(collection, nameof(collection));
            
            collection.AddHttpClient<ITMNFService, TMNFService>(ConfigureHttpClient);
            collection.AddHttpClient<ITM2Service, TM2Service>(ConfigureHttpClient);
            collection.AddHttpClient<ITM2020Service, TM2020Service>(ConfigureHttpClient);
            
            collection.AddSingleton<IDiscordMessageSubscriberRegistry>(services =>
            {
                var registry = new DiscordMessageSubscriberRegistry(services);
                registry.AddSubscriber<TMNFInfoHandler>();
                registry.AddSubscriber<TM2InfoHandler>();
                registry.AddSubscriber<TM2020InfoHandler>();
                
                return registry;
            });

            return collection;
        }

        private static void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(1);
            httpClient.DefaultRequestHeaders.Add("user-agent", "SkyTrackmaniaBot");
        }
    }
}