using Reddit.Controllers.EventArgs;
using System.Threading.Tasks;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IRedditMessageHandler
    {
        Task OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e);
    }
}