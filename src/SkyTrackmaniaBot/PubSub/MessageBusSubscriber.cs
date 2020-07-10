using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SkyTrackmaniaBot.Common.Enums;
using Validation;

namespace SkyTrackmaniaBot.PubSub
{
    public abstract class MessageBusSubscriber
    {
        protected readonly string _target;
        protected readonly MessageBusSubscriberType _type;

        protected MessageBusSubscriber(string target, MessageBusSubscriberType type)
        {
            Requires.NotNull(target, nameof(target));

            _target = target;
            _type = type;
        }

        public abstract bool ShouldGetCalled(string target, out object[] args);
        public abstract Task Invoke(object message, object[] args, CancellationToken cancellationToken = default);
    }

    public class MessageBusSubscriber<TMessage> : MessageBusSubscriber where TMessage : class
    {
        private readonly Func<TMessage, object[], CancellationToken, Task> _callback;

        public MessageBusSubscriber(string target, MessageBusSubscriberType type,
            Func<TMessage, object[], CancellationToken, Task> callback)
            : base(target, type)
        {
            Requires.NotNull(callback, nameof(callback));

            _callback = callback;
        }

        public override bool ShouldGetCalled(string target, out object[] args)
        {
            if (target == null)
            {
                args = new object[]{};
                return false;
            }
            
            if (_type == MessageBusSubscriberType.Text)
            {
                args = new object[]{};
                return _target == target;
            }

            if (_type == MessageBusSubscriberType.Regex)
            {
                var match = Regex.Match(target, _target);
                args = new object[] { match };
                return match.Success;
            }

            if (_type == MessageBusSubscriberType.StartsWith)
            {
                args = new object[]{};
                return target.StartsWith(_target);
            }
            
            args = new object[]{};
            return false;
        }

        public override Task Invoke(object message, object[] args, CancellationToken cancellationToken = default)
        {
            if (!(message is TMessage tMessage))
                return Task.CompletedTask;

            return _callback.Invoke(tMessage, args, cancellationToken);
        }
    }
}