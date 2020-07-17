using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot.Reddit
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
            _redditClientWrapper.UnreadUpdated += async (s, e) => await _redditMessageHandler.OnUnreadMessagesUpdated(s, e);
            _redditClientWrapper.MonitorUnread();
        }
    }
}
