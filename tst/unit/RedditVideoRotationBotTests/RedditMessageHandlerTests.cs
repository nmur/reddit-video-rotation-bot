using Reddit.Controllers.EventArgs;
using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditMessageHandlerTests
    {
        [Fact]
        public void GivenRedditMessageHandler_WhenOnUnreadMessagesUpdatedIsCalledWithOneUserMention_ThenNoUserMentionsWereHandled()
        {
            // Arrange
            var redditMessageHandler = new RedditMessageHandler();

            // Act
            redditMessageHandler.OnUnreadMessagesUpdated(new object(), new MessagesUpdateEventArgs());

            // Assert
            Assert.Equal(0, redditMessageHandler.TempUserMentionCount);
        }
    }
}
