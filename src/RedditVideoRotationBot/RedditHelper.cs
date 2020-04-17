using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class RedditHelper
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private readonly IRedditMessageHandler _redditMessageHandler;

        public RedditHelper(IRedditClientWrapper redditClientWrapper, IRedditMessageHandler redditMessageHandler)
        {
            _redditClientWrapper = redditClientWrapper;
            _redditMessageHandler = redditMessageHandler;
        }

        public void MonitorUnreadMessages()
        {
            _redditClientWrapper.UnreadUpdated += _redditMessageHandler.OnUnreadMessagesUpdated;
            _redditClientWrapper.MonitorUnread();
        }
    }
}
