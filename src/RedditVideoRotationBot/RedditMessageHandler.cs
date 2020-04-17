using Reddit.Controllers.EventArgs;
using RedditVideoRotationBot.Interfaces;
using System;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        public void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            Console.WriteLine($"Message received from {e.NewMessages[0].Author}");
        }
    }
}