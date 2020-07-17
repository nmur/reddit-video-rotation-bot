using System;

namespace RedditVideoRotationBot.Exceptions
{
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