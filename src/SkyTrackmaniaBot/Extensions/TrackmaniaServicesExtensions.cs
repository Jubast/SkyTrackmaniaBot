using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
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
            return collection;
        }

        private static void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(1);
            httpClient.DefaultRequestHeaders.Add("user-agent", "SkyTrackmaniaBot");
        }
    }
}