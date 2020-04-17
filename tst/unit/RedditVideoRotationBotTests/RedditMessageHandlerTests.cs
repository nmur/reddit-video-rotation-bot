using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot;
using System.Collections.Generic;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditMessageHandlerTests
    {
        [Fact]
        public void GivenRedditMessageHandler_WhenOneUnreadMessagesUpdatedIsCalledWithAnEmptyMessage_ThenNoUsernameMentionsWereHandled()
        {
            // Arrange
            var redditMessageHandler = new RedditMessageHandler();

            // Act
            redditMessageHandler.OnUnreadMessagesUpdated(new object(), new MessagesUpdateEventArgs());

            // Assert
            Assert.Equal(0, redditMessageHandler.TempUserMentionCount);
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOneUnreadMessagesUpdatedIsCalledWithNoMessages_ThenNoUsernameMentionsWereHandled()
        {
            // Arrange
            var redditMessageHandler = new RedditMessageHandler();
            MessagesUpdateEventArgs messagesUpdateEventArgs = GetMessageUpdateEventArgsWithNoMessages();

            // Act
            redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(0, redditMessageHandler.TempUserMentionCount);
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOneUnreadMessagesUpdatedIsCalledWithOneUsernameMention_ThenUsernameMentionWasHandled()
        {
            // Arrange
            var redditMessageHandler = new RedditMessageHandler();
            MessagesUpdateEventArgs messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(1, redditMessageHandler.TempUserMentionCount);
        }

        private static MessagesUpdateEventArgs GetMessageUpdateEventArgsWithNoMessages()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message>()
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessage()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetUsernameMentionMessage() }
            };
        }

        private static Message GetUsernameMentionMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "username mention",
                WasComment = true
            };
        }
    }
}
