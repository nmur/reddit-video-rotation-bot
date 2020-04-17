using Reddit.Controllers.EventArgs;
using System;

namespace RedditVideoRotationBot
{
    public interface IRedditClientWrapper
    {
        event EventHandler<MessagesUpdateEventArgs> UnreadUpdated;

        void MonitorUnread();
    }
}