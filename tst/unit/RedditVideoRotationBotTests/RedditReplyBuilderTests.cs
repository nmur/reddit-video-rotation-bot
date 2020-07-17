using RedditVideoRotationBot;
using Xunit;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using RedditVideoRotationBot.Exceptions;

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

        [Fact]
        public void GivenUploadedVideoUrlAndRotationDescription_WhenRedditReplyIsBuilt_ThenRedditReplyIsReturnedSuccessfully()
        {
            // Arrange
            var redditReplyBuilder = new RedditReplyBuilder();
            var replyBuilderParameters = new ReplyBuilderParameters
            {
                UploadedVideoUrl = UploadedVideoUrl,
                RotationDescription = RotationDescription
            };

            // Act
            var reply = redditReplyBuilder.BuildReply(replyBuilderParameters);

            // Assert
            Assert.Equal(CompletedReply, reply);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenAnInvalidUploadedVideoUrlAndValidRotationDescription_WhenRedditReplyIsBuilt_ThenRedditReplyBuilderExceptionIsThrown(string invalidUrl)
        {
            // Arrange
            var redditReplyBuilder = new RedditReplyBuilder();
            var replyBuilderParameters = new ReplyBuilderParameters
            {
                UploadedVideoUrl = invalidUrl,
                RotationDescription = RotationDescription
            };

            // Act + Assert
            Invoking(() => redditReplyBuilder.BuildReply(replyBuilderParameters))
                .Should().Throw<RedditReplyBuilderException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenValidUploadedVideoUrlAndInvalidRotationDescription_WhenRedditReplyIsBuilt_ThenRedditReplyBuilderExceptionIsThrown(string invalidRotationDescription)
        {
            // Arrange
            var redditReplyBuilder = new RedditReplyBuilder();
            var replyBuilderParameters = new ReplyBuilderParameters
            {
                UploadedVideoUrl = UploadedVideoUrl,
                RotationDescription = invalidRotationDescription
            };

            // Act + Assert
            Invoking(() => redditReplyBuilder.BuildReply(replyBuilderParameters))
                .Should().Throw<RedditReplyBuilderException>();
        }
    }
}
