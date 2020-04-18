using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Interfaces;
using System.Collections.Generic;
using FakeItEasy;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditMessageHandlerTests
    {
        private readonly IRedditClientWrapper _fakeRedditClientWrapper;

        private readonly IRedditMessageHandler _redditMessageHandler;

        public RedditMessageHandlerTests()
        {
            _fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            _redditMessageHandler = new RedditMessageHandler(_fakeRedditClientWrapper);
        }

        [Theory]
        [MemberData(nameof(MessagesUpdateEventArgsWithNoValidUsernameMentions))]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithNoValidUsernameMentions_ThenNoUsernameMentionsWereHandled(MessagesUpdateEventArgs messagesUpdateEventArgs)
        {
            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(0, _redditMessageHandler.TempUserMentionCount);
        }

        public static IEnumerable<object[]> MessagesUpdateEventArgsWithNoValidUsernameMentions =>
            new List<object[]>
            {
                new object[] { new MessagesUpdateEventArgs() },
                new object[] { GetMessageUpdateEventArgsWithNoMessages() },
                new object[] { GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage() }
            };

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMention_ThenUsernameMentionWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(1, _redditMessageHandler.TempUserMentionCount);
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithTwoUsernameMentions_ThenUsernameMentionsWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithTwoUsernameMentionMessages();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(2, _redditMessageHandler.TempUserMentionCount);
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMentionAndOneOtherMessage_ThenOneUsernameMentionWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOneOtherMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            Assert.Equal(1, _redditMessageHandler.TempUserMentionCount);
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneMessage_ThenMessageWasMarkedAsRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessages()).MustHaveHappenedOnceExactly();
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

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithTwoUsernameMentionMessages()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> {
                    GetUsernameMentionMessage(),
                    GetUsernameMentionMessage()
                }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOneOtherMessage()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> {
                    GetUsernameMentionMessage(),
                    GetNonUsernameMentionMessage()
                }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetNonUsernameMentionMessage() }
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

        private static Message GetNonUsernameMentionMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "hey!",
                WasComment = false
            };
        }
    }
}
