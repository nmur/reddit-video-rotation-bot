using System;

namespace RedditVideoRotationBot.Exceptions
{
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