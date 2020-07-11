#nullable enable
using System;

namespace SkyTrackmaniaBot.Common.Attributes
{
    public class CommandSubscriptionAttribute : Attribute
    {
        public CommandSubscriptionAttribute(string? command = null, string? prefix = null)
        {
            Command = command;
            Prefix = prefix;
        }

        public string? Command { get; }

        public string? Prefix { get; }
    }
}