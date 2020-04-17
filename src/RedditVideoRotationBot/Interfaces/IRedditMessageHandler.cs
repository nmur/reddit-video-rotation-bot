using Reddit.Controllers.EventArgs;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IRedditMessageHandler
    {
        void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e);
    }
}