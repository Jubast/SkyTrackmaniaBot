using System;
using Validation;

namespace SkyTrackmaniaBot.Common.Attributes
{
    public class RegexSubscriptionAttribute : Attribute
    {
        public RegexSubscriptionAttribute(string pattern)
        {
            Requires.NotNull(pattern, nameof(pattern));
            Pattern = pattern;
        }

        public string Pattern { get; }
    }
}