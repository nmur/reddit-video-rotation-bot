using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RedditPostParserException : Exception
    {
        public RedditPostParserException()
        {
        }

        public RedditPostParserException(string message)
            : base(message)
        {
        }

        public RedditPostParserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}