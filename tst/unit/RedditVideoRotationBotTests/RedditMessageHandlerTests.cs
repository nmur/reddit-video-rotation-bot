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

        private const string PrivateMessageId = "abcabc";

        private const string PrivateMessageFullname = "t4_" + PrivateMessageId;

        private const string CommentReplyId = "abcabc";

        private const string CommentReplyFullname = "t4_" + CommentReplyId;

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
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(A<string>._)).MustNotHaveHappened();
        }

        public static IEnumerable<object[]> MessagesUpdateEventArgsWithNoValidUsernameMentions =>
            new List<object[]>
            {
                new object[] { new MessagesUpdateEventArgs() },
                new object[] { GetMessageUpdateEventArgsWithNoMessages() },
            };

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOnePrivateMessage_ThenMessageWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(PrivateMessageFullname)).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMentionAndOnePrivateMessage_ThenOneUsernameMentionAndOnePrivateMessageWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOnePrivateMessage();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(PrivateMessageFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(PrivateMessageFullname)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithAMixOfMessages_ThenMessagesWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithAMixOfMessages();

            // Act
            _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(PrivateMessageFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(PrivateMessageFullname)).MustNotHaveHappened();
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(CommentReplyFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(CommentReplyFullname)).MustNotHaveHappened();
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

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOnePrivateMessage()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> {
                    GetUsernameMentionMessage(),
                    GetPrivateMessage()
                }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithAMixOfMessages()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> {
                    GetUsernameMentionMessage(),
                    GetPrivateMessage(),
                    GetCommentReplyMessage()
                }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetPrivateMessage() }
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

        private static Message GetPrivateMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "hey!",
                WasComment = false,
                Id = PrivateMessageId
            };
        }

        private static Message GetCommentReplyMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "comment reply",
                WasComment = true,
                Id = CommentReplyId
            };
        }
    }
}
