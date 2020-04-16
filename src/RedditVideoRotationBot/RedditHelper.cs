using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System;

namespace RedditVideoRotationBot
{
    public class RedditHelper
    {
        public void ReadMessages()
        {
            var reddit = new RedditClient();
            Console.WriteLine($"RedditClient initialized for {reddit.Account.Me.Name}");

            MonitorMessages(reddit.Account.Messages);
        }

        private void MonitorMessages(PrivateMessages messages)
        {
            messages.MonitorUnread();
            messages.UnreadUpdated += C_UnreadMessagesUpdated;

            DateTime start = DateTime.Now;
            while (start.AddMilliseconds(600000) > DateTime.Now)
            {
            }

            messages.UnreadUpdated -= C_UnreadMessagesUpdated;
            messages.MonitorUnread();
        }

        private void C_UnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            Console.WriteLine($"{e.NewMessages.Count} messages received.");
            foreach (var message in e.NewMessages)
            {
                Console.WriteLine($"Message received at {message.CreatedUTC.ToLocalTime()}.");
                if (message.Subject == "username mention")
                {
                    Console.WriteLine($"Senpai noticed me! - {message.Author}");
                }
            }
        }
    }
}
