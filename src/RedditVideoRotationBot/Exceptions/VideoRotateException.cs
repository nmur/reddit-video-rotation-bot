using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VideoRotateException : Exception
    {
        public VideoRotateException()
        {
        }

        public VideoRotateException(string message)
            : base(message)
        {
        }

        public VideoRotateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}