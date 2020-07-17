using RedditVideoRotationBot;
using Xunit;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using RedditVideoRotationBot.Media.Ffmpeg;
using RedditVideoRotationBot.Reddit;

namespace RedditVideoRotationBotTests
{
    public class RedditReplyBuilderTests
    {
        private readonly string CompletedReply = $"Video was rotated {RotationDescription}: {UploadedVideoUrl}  \r\n\r\n***\r\n^^[usage]({UsageUrl})&nbsp;-&nbsp;[source]({SourceUrl})&nbsp;-&nbsp;[pm&nbsp;me]({PmUrl})";

        private const string UploadedVideoUrl = "https://giant.gfycat.com/SomeFakeVideo.mp4";

        private const string UsageUrl = "https://github.com/nmur/reddit-video-rotation-bot/wiki/Detailed-usage-instructions";

        private const string SourceUrl = "https://github.com/nmur/reddit-video-rotation-bot";

        private const string PmUrl = "https://www.reddit.com/message/compose/?to=nmur";

        private const string RotationDescription = "90° clockwise";

        private const string RotationMessageArg = "cw";

        private readonly IReplyBuilder _redditReplyBuilder;

        public RedditReplyBuilderTests()
        {
            _redditReplyBuilder = new RedditReplyBuilder(new FfmpegRotationDescriptionDeterminer());
        }

        [Fact]
        public void GivenUploadedVideoUrlAndRotationMessageArg_WhenRedditReplyIsBuilt_ThenRedditReplyIsReturnedSuccessfully()
        {
            // Arrange
            var replyBuilderParameters = new RedditReplyBuilderParameters
            {
                UploadedVideoUrl = UploadedVideoUrl,
                RotationMessageArg = RotationMessageArg
            };

            // Act
            var reply = _redditReplyBuilder.BuildReply(replyBuilderParameters);

            // Assert
            Assert.Equal(CompletedReply, reply);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenAnInvalidUploadedVideoUrlAndValidRotationMessageArg_WhenRedditReplyIsBuilt_ThenRedditReplyBuilderExceptionIsThrown(string invalidUrl)
        {
            // Arrange
            var replyBuilderParameters = new RedditReplyBuilderParameters
            {
                UploadedVideoUrl = invalidUrl,
                RotationMessageArg = RotationMessageArg
            };

            // Act + Assert
            Invoking(() => _redditReplyBuilder.BuildReply(replyBuilderParameters))
                .Should().Throw<RedditReplyBuilderException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenValidUploadedVideoUrlAndInvalidRotationMessageArg_WhenRedditReplyIsBuilt_ThenRedditReplyBuilderExceptionIsThrown(string invalidRotationMessageArg)
        {
            // Arrange
            var replyBuilderParameters = new RedditReplyBuilderParameters
            {
                UploadedVideoUrl = UploadedVideoUrl,
                RotationMessageArg = invalidRotationMessageArg
            };

            // Act + Assert
            Invoking(() => _redditReplyBuilder.BuildReply(replyBuilderParameters))
                .Should().Throw<RedditReplyBuilderException>();
        }
    }
}
