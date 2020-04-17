using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        public int TempUserMentionCount; //TODO: remove this count once we have a state that's testable 

        public void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            foreach (var message in e.NewMessages)
            {
                Console.WriteLine($"Message received from {message.Author}");

                if (MessageIsUserMention(message))
                {
                    TempUserMentionCount++;
                    Console.WriteLine($"Message was a user mention");
                }
            }
        }

        private static bool MessageIsUserMention(Message message)
        {
            return message.Subject == "user mention" && message.WasComment;
        }
    }
}