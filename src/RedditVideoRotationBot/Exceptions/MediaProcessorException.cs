using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MediaProcessorException : Exception
    {
        public MediaProcessorException()
        {
        }

        public MediaProcessorException(string message) 
            : base(message)
        {
        }

        public MediaProcessorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}