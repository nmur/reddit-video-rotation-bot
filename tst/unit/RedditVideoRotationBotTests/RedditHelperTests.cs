using FakeItEasy;
using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditHelperTests
    {
        [Fact]
        public void WhenRedditHelperIsConstructed_ThenNoExceptionIsThrown()
        {
            var fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            new RedditHelper(fakeRedditClientWrapper);
        }

        [Fact]
        public void GivenRedditHelper_WhenMonitorUnreadMessagesIsCalled_ThenUnreadMessagesAreMonitored()
        {
            // Arrange
            var fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            var redditHelper = new RedditHelper(fakeRedditClientWrapper);

            // Act
            redditHelper.MonitorUnreadMessages();

            // Assert
            A.CallTo(() => fakeRedditClientWrapper.MonitorUnread()).MustHaveHappenedOnceExactly();
        }
    }
}
