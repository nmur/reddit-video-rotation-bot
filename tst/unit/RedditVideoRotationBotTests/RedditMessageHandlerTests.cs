using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Interfaces;
using System.Collections.Generic;
using FakeItEasy;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using RedditVideoRotationBot.Exceptions;

namespace RedditVideoRotationBotTests
{
    public class RedditMessageHandlerTests
    {
        private const string UsernameMentionId = "xyzxyz";

        private const string UsernameMentionFullname = "t1_" + UsernameMentionId;

        private const string UsernameMentionWithNoMediaId = "defdef";

        private const string UsernameMentionWithNoMediaFullname = "t1_" + UsernameMentionWithNoMediaId;

        private const string PrivateMessageId = "abcabc";

        private const string PrivateMessageFullname = "t4_" + PrivateMessageId;

        private const string CommentReplyId = "abcabc";

        private const string CommentReplyFullname = "t1_" + CommentReplyId;

        private const string UsernameMentionOnNsfwPostId = "qwerty";

        private const string UsernameMentionOnNsfwPostFullname = "t1_" + UsernameMentionOnNsfwPostId;

        private const string VideoUrlString = "https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback";

        private const string AudioUrlString = "https://v.redd.it/abcabcabcabc/audio";

        private const string MediaString = "{\"reddit_video\":{\"fallback_url\":\"https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback\",\"height\":1080,\"width\":608,\"scrubber_media_url\":\"https://v.redd.it/abcabcabcabc/DASH_96\",\"dash_url\":\"https://v.redd.it/abcabcabcabc/DASHPlaylist.mpd\",\"duration\":8,\"hls_url\":\"https://v.redd.it/abcabcabcabc/HLSPlaylist.m3u8\",\"is_gif\":false,\"transcoding_status\":\"completed\"}}";

        private readonly IRedditClientWrapper _fakeRedditClientWrapper;

        private readonly IVideoDownloader _fakeVideoDownloader;

        private readonly IAudioDownloader _fakeAudioDownloader;

        private readonly IMediaProcessor _fakeMediaProcessor;

        private readonly IVideoRotator _fakeVideoRotator;

        private readonly IRedditMessageHandler _redditMessageHandler;

        private readonly IVideoUploader _fakeGfyCatVideoUploader;

        public RedditMessageHandlerTests()
        {
            _fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            SetupCommentRootPostStubs();
            _fakeVideoDownloader = A.Fake<IVideoDownloader>();
            _fakeAudioDownloader = A.Fake<IAudioDownloader>();
            _fakeMediaProcessor = A.Fake<IMediaProcessor>();
            _fakeVideoRotator = A.Fake<IVideoRotator>();
            _fakeGfyCatVideoUploader = A.Fake<IVideoUploader>();
            _redditMessageHandler = new RedditMessageHandler(_fakeRedditClientWrapper, _fakeVideoDownloader, _fakeAudioDownloader, _fakeMediaProcessor, _fakeVideoRotator, _fakeGfyCatVideoUploader);
        }

        [Theory]
        [MemberData(nameof(MessagesUpdateEventArgsWithNoValidUsernameMentions))]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithNoValidUsernameMentions_ThenNoUsernameMentionsWereHandled(MessagesUpdateEventArgs messagesUpdateEventArgs)
        {
            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        public static IEnumerable<object[]> MessagesUpdateEventArgsWithNoValidUsernameMentions =>
            new List<object[]>
            {
                new object[] { new MessagesUpdateEventArgs() },
                new object[] { GetMessageUpdateEventArgsWithNoMessages() },
            };

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOnePrivateMessage_ThenMessageWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneNonUsernameMentionMessage();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(PrivateMessageFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMention_ThenUsernameMentionWasHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(1);
            AssertOneUsernameMentionWasMarkedReadAndRepliedTo();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithTwoUsernameMentions_ThenTwoUsernameMentionsWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithTwoUsernameMentionMessages();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(2);
            AssertNumberOfRepliedToComments(2);
            AssertTwoUsernameMentionsWereMarkedReadAndRepliedTo();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUsernameMentionAndOnePrivateMessage_ThenOneUsernameMentionAndOnePrivateMessageWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageAndOnePrivateMessage();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(2);
            AssertNumberOfRepliedToComments(1);
            AssertOneUsernameMentionWasMarkedReadAndRepliedTo();
            AssertOnePrivateMessageWasMarkedRead();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithAMixOfMessages_ThenMessagesWereHandled()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithAMixOfMessages();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(3);
            AssertNumberOfRepliedToComments(1);
            AssertOneUsernameMentionWasMarkedReadAndRepliedTo();
            AssertOnePrivateMessageWasMarkedRead();
            AssertOneCommentReplyWasMarkedRead();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenNoVideoUrlWasFoundInPost_ThenNoVideoIsProcessedAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageOnPostWithNoMedia();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenVideoDownloadFails_ThenNoVideoIsProcessedAndCommentWasNotRepliedToAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).Throws<VideoDownloadException>();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenVideoRotateFails_ThenNoVideoIsProcessedAndCommentWasNotRepliedToAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).Throws<VideoRotateException>();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenVideoUploadFails_ThenNoVideoIsProcessedAndCommentWasNotRepliedToAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).Throws<VideoUploadException>();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithUsernameMentionOnNsfwMediaPost_ThenNoVideoIsProcessedAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageOnPostMarkedAsNsfw();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithUsernameMentionWithNoRotationArgument_ThenNoVideoIsProcessedAndCommentWasMarkedRead()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessageWithNoRotationArgument();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            AssertNumberOfReadMessages(1);
            AssertNumberOfRepliedToComments(0);
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaProcessor.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithUsernameMentionNoRotationArgument_ThenVideoRotatorIsGivenRotationArgument()
        {
            // Arrange
            var messagesUpdateEventArgs = GetMessagesUpdateEventArgsWithOneUsernameMentionMessage();

            // Act
            await _redditMessageHandler.OnUnreadMessagesUpdated(new object(), messagesUpdateEventArgs);

            // Assert
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).MustHaveHappened();
        }

        private void SetupCommentRootPostStubs()
        {
            A.CallTo(() => _fakeRedditClientWrapper.GetCommentRootPost(UsernameMentionFullname)).Returns(GetPostWithVideoMedia());
            A.CallTo(() => _fakeRedditClientWrapper.GetCommentRootPost(PrivateMessageFullname)).Returns(GetPostWithNoMedia());
            A.CallTo(() => _fakeRedditClientWrapper.GetCommentRootPost(CommentReplyFullname)).Returns(GetPostWithVideoMedia());
            A.CallTo(() => _fakeRedditClientWrapper.GetCommentRootPost(UsernameMentionWithNoMediaFullname)).Returns(GetPostWithNoMedia());
            A.CallTo(() => _fakeRedditClientWrapper.GetCommentRootPost(UsernameMentionOnNsfwPostFullname)).Returns(GetNsfwPostWithVideoMedia());
        }

        private static Reddit.Controllers.Post GetPostWithVideoMedia()
        {
            return new Reddit.Controllers.Post(null, new Post { Media = JObject.Parse(MediaString) });
        }

        private static Reddit.Controllers.Post GetPostWithNoMedia()
        {
            return new Reddit.Controllers.Post(null, new Post());
        }

        private static Reddit.Controllers.Post GetNsfwPostWithVideoMedia()
        {
            return new Reddit.Controllers.Post(null, new Post { Media = JObject.Parse(MediaString), Over18 = true });
        }

        private void AssertOneUsernameMentionWasMarkedReadAndRepliedTo()
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname, A<string>._)).MustHaveHappenedOnceExactly();
        }

        private void AssertTwoUsernameMentionsWereMarkedReadAndRepliedTo()
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(UsernameMentionFullname)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(UsernameMentionFullname, A<string>._)).MustHaveHappenedTwiceExactly();
        }

        private void AssertOnePrivateMessageWasMarkedRead()
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(PrivateMessageFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(PrivateMessageFullname, A<string>._)).MustNotHaveHappened();
        }

        private void AssertOneCommentReplyWasMarkedRead()
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(CommentReplyFullname)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(CommentReplyFullname, A<string>._)).MustNotHaveHappened();
        }

        private void AssertNumberOfReadMessages(int times)
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReadMessage(A<string>._)).MustHaveHappened(times, Times.Exactly);
        }

        private void AssertNumberOfRepliedToComments(int times)
        {
            A.CallTo(() => _fakeRedditClientWrapper.ReplyToComment(A<string>._, A<string>._)).MustHaveHappened(times, Times.Exactly);
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

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessageOnPostWithNoMedia()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetUsernameMentionWithNoMediaMessage() }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessageOnPostMarkedAsNsfw()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetUsernameMentionWithNsfwMessage() }
            };
        }

        private static MessagesUpdateEventArgs GetMessagesUpdateEventArgsWithOneUsernameMentionMessageWithNoRotationArgument()
        {
            return new MessagesUpdateEventArgs
            {
                NewMessages = new List<Message> { GetUsernameMentionWithNoRotationArgument() }
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
                Id = UsernameMentionId,
                Body = "/u/bot cw"
            };
        }

        private static Message GetUsernameMentionWithNoMediaMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "username mention",
                WasComment = true,
                Id = UsernameMentionWithNoMediaId,
                Body = "/u/bot cw"
            };
        }

        private static Message GetUsernameMentionWithNsfwMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "username mention",
                WasComment = true,
                Id = UsernameMentionOnNsfwPostId,
                Body = "/u/bot cw"
            };
        }

        private static Message GetUsernameMentionWithNoRotationArgument()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "username mention",
                WasComment = true,
                Id = UsernameMentionOnNsfwPostId,
                Body = "/u/bot"
            };
        }

        private static Message GetPrivateMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "hey!",
                WasComment = false,
                Id = PrivateMessageId,
                Body = "/u/bot cw"
            };
        }

        private static Message GetCommentReplyMessage()
        {
            return new Message
            {
                Author = "test-user",
                Subject = "comment reply",
                WasComment = true,
                Id = CommentReplyId,
                Body = "/u/bot cw"
            };
        }
    }
}
