using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditReplyBuilderTests
    {
        private const string UploadedVideoUrl = "https://giant.gfycat.com/SomeFakeVideo.mp4";

        [Fact]
        public void GivenUploadedVideoUrl_WhenRedditReplyIsBuilt_ThenRedditReplyIsReturnedSuccessfully()
        {
            // Arrange
            var redditReplyBuilder = new RedditReplyBuilder();
            var expectedReply = $"Rotated Video: {UploadedVideoUrl}  \r\n\r\n***\r\n^^[usage](https://github.com/nmur/reddit-video-rotation-bot/wiki/Detailed-usage-instructions)&nbsp;-&nbsp;[source](https://github.com/nmur/reddit-video-rotation-bot)&nbsp;-&nbsp;[pm&nbsp;me](https://www.reddit.com/message/compose/?to=nmur)";

            // Act
            var reply = redditReplyBuilder.BuildReply(UploadedVideoUrl);

            // Assert
            Assert.Equal(expectedReply, reply);
        }
    }
}
