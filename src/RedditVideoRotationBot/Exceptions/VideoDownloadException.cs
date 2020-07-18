using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
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