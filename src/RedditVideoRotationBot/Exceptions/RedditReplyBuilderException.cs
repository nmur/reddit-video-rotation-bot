using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RedditReplyBuilderException : Exception
    {
        public RedditReplyBuilderException()
        {
        }

        public RedditReplyBuilderException(string message)
            : base(message)
        {
        }

        public RedditReplyBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}