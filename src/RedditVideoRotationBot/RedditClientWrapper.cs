﻿using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage] // Wrapper for RedditClient which has no Interface for unit testing
    public class RedditClientWrapper : IRedditClientWrapper
    {
        private readonly RedditClient _redditClient;

        public RedditClientWrapper(IRedditClientConfiguration redditClientConfiguration)
        {
            Console.WriteLine($"Creating RedditClient...");
            _redditClient = new RedditClient(
                redditClientConfiguration.GetAppId(),
                redditClientConfiguration.GetRefreshToken(),
                redditClientConfiguration.GetAppSecret());
            Console.WriteLine($"RedditClient created for user: {_redditClient.Account.Me.Name}");
        }

        public event EventHandler<MessagesUpdateEventArgs> UnreadUpdated;

        public void MonitorUnread()
        {
            MonitorUnreadMessages(_redditClient.Account.Messages);
        }

        // This method deals with https://github.com/sirkris/Reddit.NET/issues/105
        private void MonitorUnreadMessages(PrivateMessages messages)
        {
            messages.MonitorUnread();
            messages.UnreadUpdated += InvokeUnreadUpdatedEvent;
        }

        private void InvokeUnreadUpdatedEvent(object sender, MessagesUpdateEventArgs e)
        {
            UnreadUpdated.Invoke(sender, e);
        }
    }
}