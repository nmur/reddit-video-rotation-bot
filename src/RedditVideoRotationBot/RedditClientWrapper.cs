using Reddit;
using System;

namespace RedditVideoRotationBot
{
    public class RedditClientWrapper : IRedditClientWrapper
    {
        private RedditClient RedditClient;

        public RedditClientWrapper(IRedditClientConfiguration redditClientConfiguration)
        {
            Console.WriteLine($"Creating RedditClient...");
            RedditClient = new RedditClient(
                redditClientConfiguration.GetAppId(),
                redditClientConfiguration.GetRefreshToken(),
                redditClientConfiguration.GetAppSecret());
            Console.WriteLine($"RedditClient created for user: {RedditClient.Account.Me.Name}");
        }
    }
}