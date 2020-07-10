using System;
using Validation;

namespace SkyTrackmaniaBot.Common.Attributes
{
    public class CommandSubscriptionAttribute : Attribute
    {
        public CommandSubscriptionAttribute(string prefix)
        {
            Requires.NotNull(prefix, nameof(prefix));
            Prefix = prefix;
        }

        public string Prefix { get; }
    }
}