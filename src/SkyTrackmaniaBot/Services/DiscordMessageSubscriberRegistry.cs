using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using SkyTrackmaniaBot.Common.Attributes;
using SkyTrackmaniaBot.Common.Enums;
using SkyTrackmaniaBot.Common.Interfaces;
using SkyTrackmaniaBot.PubSub;
using Validation;

namespace SkyTrackmaniaBot.Services
{
    public class DiscordMessageSubscriberRegistry : IDiscordMessageSubscriberRegistry
    {
        private readonly MessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;

        public DiscordMessageSubscriberRegistry(IServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
            _messageBus = new MessageBus();
        }

        public Task PublishMessageCreated(MessageCreateEventArgs messageCreated,
            CancellationToken cancellationToken = default)
        {
            return _messageBus.Publish(messageCreated.Message.Content, messageCreated, cancellationToken);
        }

        public void AddSubscriber<TSubscriber>() where TSubscriber : class, IDiscordMessageSubscriber
        {
            var type = typeof(TSubscriber);

            var regexAttribute = type.GetCustomAttribute<RegexSubscriptionAttribute>(false);
            if (regexAttribute != null)
            {
                _messageBus.Subscribe(regexAttribute.Pattern, MessageBusSubscriberType.Regex, CreateCallback<TSubscriber>());
            }
            
            var commandAttribute = type.GetCustomAttribute<CommandSubscriptionAttribute>(false);
            if (commandAttribute != null)
            {
                if(commandAttribute.Command != null)
                    _messageBus.Subscribe(commandAttribute.Command, MessageBusSubscriberType.Text, CreateCallback<TSubscriber>());
                
                if(commandAttribute.Prefix != null)
                    _messageBus.Subscribe(commandAttribute.Prefix, MessageBusSubscriberType.StartsWith, CreateCallback<TSubscriber>());
            }
        }

        private Func<MessageCreateEventArgs, object[], CancellationToken, Task> CreateCallback<TSubscriber>()
            where TSubscriber : class, IDiscordMessageSubscriber
        {
            return (message, args, token) => CreateSubscriber<TSubscriber>(args).OnMessageCreated(message, token);
        }

        private TSubscriber CreateSubscriber<TSubscriber>(object[] args) where TSubscriber : class, IDiscordMessageSubscriber
        {
            return ActivatorUtilities.CreateInstance<TSubscriber>(_serviceProvider, args);
        }
    }
}