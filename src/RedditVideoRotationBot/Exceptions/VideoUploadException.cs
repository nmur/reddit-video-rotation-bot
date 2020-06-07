using System;

namespace RedditVideoRotationBot.Exceptions
{
    public class VideoUploadException : Exception
    {
        public VideoUploadException()
        {
        }

        public VideoUploadException(string message)
            : base(message)
        {
        }

        public VideoUploadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}