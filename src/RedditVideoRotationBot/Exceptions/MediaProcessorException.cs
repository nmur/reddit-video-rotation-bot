using System;

namespace RedditVideoRotationBot.Exceptions
{
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