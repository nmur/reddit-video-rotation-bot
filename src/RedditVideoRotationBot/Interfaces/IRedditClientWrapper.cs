using Reddit.Controllers.EventArgs;
using System;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IRedditClientWrapper
    {
        event EventHandler<MessagesUpdateEventArgs> UnreadUpdated;

        void MonitorUnread();

        void ReadMessage(string id);

        void ReplyToComment(string id);
    }
}