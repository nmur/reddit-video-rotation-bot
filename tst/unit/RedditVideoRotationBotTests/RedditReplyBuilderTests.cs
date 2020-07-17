using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditReplyBuilderTests
    {
        private readonly string CompletedReply = $"Rotated Video: {UploadedVideoUrl}  \r\n\r\n***\r\n^^[usage]({UsageUrl})&nbsp;-&nbsp;[source]({SourceUrl})&nbsp;-&nbsp;[pm&nbsp;me]({PmUrl})";

        private const string UploadedVideoUrl = "https://giant.gfycat.com/SomeFakeVideo.mp4";

        private const string UsageUrl = "https://github.com/nmur/reddit-video-rotation-bot/wiki/Detailed-usage-instructions";

        private const string SourceUrl = "https://github.com/nmur/reddit-video-rotation-bot";

        private const string PmUrl = "https://www.reddit.com/message/compose/?to=nmur";

        [Fact]
        public void GivenUploadedVideoUrl_WhenRedditReplyIsBuilt_ThenRedditReplyIsReturnedSuccessfully()
        {
            // Arrange
            var redditReplyBuilder = new RedditReplyBuilder();

            // Act
            var reply = redditReplyBuilder.BuildReply(UploadedVideoUrl);

            // Assert
            Assert.Equal(CompletedReply, reply);
        }
    }
}
