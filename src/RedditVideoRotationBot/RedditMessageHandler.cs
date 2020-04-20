﻿using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private const string UsernameMentionSubjectString = "username mention";

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
                    Console.WriteLine($"Message was a user mention");
                    string videoUrl = RedditPostParser.TryGetVideoUrlFromPost(GetCommentRootPost(message));
                    Console.WriteLine($"videoUrl: {videoUrl}");

                    ReplyToComment(message);
                }

                MarkMessageAsRead(message);
            }
        }

        private static bool MessageIsUsernameMention(Message message)
        {
            return message.Subject == UsernameMentionSubjectString && message.WasComment;
        }

        private Post GetCommentRootPost(Message message)
        {
            return _redditClientWrapper.GetCommentRootPost(GetMessageFullname(message)).Listing;
        }

        private void MarkMessageAsRead(Message message)
        {
            _redditClientWrapper.ReadMessage(GetMessageFullname(message));
            Console.WriteLine($"Message was marked as read");
        }

        private void ReplyToComment(Message message)
        {
            _redditClientWrapper.ReplyToComment(GetMessageFullname(message));
            Console.WriteLine($"Comment was replied to");
        }

        private static string GetMessageFullname(Message message)
        {
            // User mentions/comments seem to require the t1_x name instead for the read_message API
            return (message.WasComment ? "t1_" : "t4_") + message.Id;
        }
    }
}