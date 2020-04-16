using Reddit;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage] // Wrapper for RedditClient which has no Interface for unit testing
    public class RedditClientWrapper : IRedditClientWrapper
    {
        private readonly RedditClient _redditClient;

        public RedditClientWrapper(IRedditClientConfiguration redditClientConfiguration)
        {
            Console.WriteLine($"Creating RedditClient...");
            _redditClient = new RedditClient(
                redditClientConfiguration.GetAppId(),
                redditClientConfiguration.GetRefreshToken(),
                redditClientConfiguration.GetAppSecret());
            Console.WriteLine($"RedditClient created for user: {_redditClient.Account.Me.Name}");
            System.Threading.Thread.Sleep(1000*60*60*24); // sleep for a day to prevent loops, for now.
        }
    }
}