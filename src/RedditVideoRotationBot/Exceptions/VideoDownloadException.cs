using System;

namespace RedditVideoRotationBot.Exceptions
{
    public class VideoDownloadException : Exception
    {
        public VideoDownloadException()
        {
        }

        public VideoDownloadException(string message)
            : base(message)
        {
        }

        public VideoDownloadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}