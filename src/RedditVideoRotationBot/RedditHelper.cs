using Reddit.Controllers.EventArgs;
using System;

namespace RedditVideoRotationBot
{
    public class RedditHelper
    {
        private readonly IRedditClientWrapper RedditClientWrapper;

        public RedditHelper(IRedditClientWrapper redditClientWrapper)
        {
            RedditClientWrapper = redditClientWrapper;
        }

        public void MonitorUnreadMessages()
        {
            RedditClientWrapper.UnreadUpdated += OnUnreadMessagesUpdated;
            RedditClientWrapper.MonitorUnread();
        }

        private void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            Console.WriteLine($"Message received from {e.NewMessages[0].Author}");
        }
    }
}
