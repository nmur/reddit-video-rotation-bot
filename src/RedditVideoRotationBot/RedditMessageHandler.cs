using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Collections.Generic;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private const string UsernameMentionSubjectString = "username mention";

        public int TempUserMentionCount { get; private set; } //TODO: remove this count once we have a state that's testable 

        public RedditMessageHandler(IRedditClientWrapper redditClientWrapper)
        {
            _redditClientWrapper = redditClientWrapper;
        }

        public void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            if (e == null || e.NewMessages == null) return;

            foreach (var message in e.NewMessages)
            {
                Console.WriteLine($"Message received from {message.Author}");

                if (MessageIsUsernameMention(message))
                {
                    TempUserMentionCount++;
                    Console.WriteLine($"Message was a user mention");
                    _redditClientWrapper.ReadMessage(message.Name); // User mentions seem to require the t1_x name instead for this API
                }
                else
                {
                    _redditClientWrapper.ReadMessage(message.Fullname);
                }
            }
        }

        private static bool MessageIsUsernameMention(Message message)
        {
            return message.Subject == UsernameMentionSubjectString && message.WasComment;
        }
    }
}