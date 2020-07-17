using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class RedditReplyBuilder : IReplyBuilder
    {
        public string BuildReply(string uploadedVideoUrl)
        {
            return uploadedVideoUrl;
        }
    }
}
