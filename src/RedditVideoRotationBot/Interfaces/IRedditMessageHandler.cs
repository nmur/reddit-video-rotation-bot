using Reddit.Controllers.EventArgs;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IRedditMessageHandler
    {
        int TempUserMentionCount { get; }

        void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e);
    }
}