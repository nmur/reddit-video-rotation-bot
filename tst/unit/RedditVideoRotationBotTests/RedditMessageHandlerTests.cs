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
        private const string UsernameMentionId = "xyzxyz";

        private const string UsernameMentionFullname = "t1_" + UsernameMentionId;

        private const string MessageId = "abcabc";

        private const string MessageFullname = "t4_" + MessageId;

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
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(A<string>._)).MustNotHaveHappened();
        }

        public static IEnumerable<object[]> MessagesUpdateEventArgsWithNoValidUsernameMentions =>
            new List<object[]>
            {
                new object[] { new MessagesUpdateEventArgs() },
                new object[] { GetMessageUpdateEventArgsWithNoMessages() },
            };

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneNonUsernameMentionMessage_ThenMessageWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(MessageFullname)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMention_ThenUsernameMentionWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithTwoUsernameMentions_ThenTwoUsernameMentionsWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithTwoUsernameMentionMessages();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMentionAndOneOtherMessage_ThenOneUsernameMentionAndOneOtherMessageWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOneOtherMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(MessageFullname)).MustHaveHappenedOnceExactly();
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
                WasComment = true,
                Id = UsernameMentionId
            };
        }

        private static Message GetNonUsernameMentionMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "hey!",
                WasComment = false,
                Id = MessageId
            };
        }
    }
}
