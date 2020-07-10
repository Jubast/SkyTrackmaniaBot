using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SkyTrackmaniaBot.Common.Enums;
using Validation;

namespace SkyTrackmaniaBot.PubSub
{
    public class MessageBus
    {
        private readonly List<MessageBusSubscriber> _subscribers = new List<MessageBusSubscriber>();

        public void Subscribe<TMessage>(string target, MessageBusSubscriberType subscriberType,
            Func<TMessage, object[], CancellationToken, Task> callback) where TMessage : class
        {
            Requires.NotNull(target, nameof(target));
            Requires.NotNull(callback, nameof(callback));

            _subscribers.Add(new MessageBusSubscriber<TMessage>(target, subscriberType, callback));
        }

        public async Task Publish<T>(string target, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            foreach (var subscriber in _subscribers)
            {
                if (subscriber.ShouldGetCalled(target, out var args))
                {
                    await subscriber.Invoke(message, args, cancellationToken);
                }
            }
        }
    }
}