namespace RedditVideoRotationBot
{
    public class RedditHelper
    {
        private readonly IRedditClientWrapper RedditClientWrapper;

        public RedditHelper(IRedditClientWrapper redditClientWrapper)
        {
            RedditClientWrapper = redditClientWrapper;
        }
    }
}
