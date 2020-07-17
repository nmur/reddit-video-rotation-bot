using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using Reddit.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace RedditVideoRotationBot.Reddit
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

        public void ReadMessage(string id)
        {
            _redditClient.Account.Messages.ReadMessage(id);
        }

        public void ReplyToComment(string id, string text)
        {
            try
            {
                _redditClient.Comment(id).Reply(text);
            }
            catch (RedditRateLimitException)
            {
                Console.WriteLine("Reddit's reply rate limit was exceeded!");
            }
        }

        //TODO: clean up/unit test/pull reddit action retries into a decorator
        public Post GetCommentRootPost(string id)
        {
            var tries = 15;
            while (tries > 0)
            {
                try
                {
                    return _redditClient.Comment(id).GetRoot(id);
                }
                catch (Exception)
                {
                    if (tries-- <= 0) break;
                    Thread.Sleep(10000);
                }
            }
            throw new Exception("Failed to find root post of comment.");
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