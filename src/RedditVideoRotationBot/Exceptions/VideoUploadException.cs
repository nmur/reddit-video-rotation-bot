using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Exceptions
{
    [ExcludeFromCodeCoverage]
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