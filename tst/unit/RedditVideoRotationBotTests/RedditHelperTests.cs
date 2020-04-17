using FakeItEasy;
using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditHelperTests
    {
        readonly IRedditClientWrapper _fakeRedditClientWrapper;

        readonly RedditHelper _redditHelper;

        public RedditHelperTests()
        {
            _fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            _redditHelper = new RedditHelper(_fakeRedditClientWrapper);
        }

        [Fact]
        public void GivenRedditHelper_WhenMonitorUnreadMessagesIsCalled_ThenUnreadMessagesAreMonitored()
        {
            // Act
            _redditHelper.MonitorUnreadMessages();

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.MonitorUnread()).MustHaveHappenedOnceExactly();
        }
    }
}
