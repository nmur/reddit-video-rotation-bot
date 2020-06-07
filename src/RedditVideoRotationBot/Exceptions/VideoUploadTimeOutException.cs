using System;

namespace RedditVideoRotationBot.Exceptions
{
    public class VideoUploadTimeOutException : VideoUploadException
    {
        public VideoUploadTimeOutException()
        {
        }

        public VideoUploadTimeOutException(string message)
            : base(message)
        {
        }

        public VideoUploadTimeOutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}