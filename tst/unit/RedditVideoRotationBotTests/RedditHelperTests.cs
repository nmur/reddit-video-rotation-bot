using FakeItEasy;
using Reddit.Controllers.EventArgs;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Interfaces;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class RedditHelperTests
    {
        private readonly IRedditClientWrapper _fakeRedditClientWrapper;

        private readonly IRedditMessageHandler _fakeRedditMessageHandler;

        private readonly RedditHelper _redditHelper;

        public RedditHelperTests()
        {
            _fakeRedditClientWrapper = A.Fake<IRedditClientWrapper>();
            _fakeRedditMessageHandler = A.Fake<IRedditMessageHandler>();
            _redditHelper = new RedditHelper(_fakeRedditClientWrapper, _fakeRedditMessageHandler);
        }

        [Fact]
        public void GivenRedditHelper_WhenMonitorUnreadMessagesIsCalled_ThenUnreadMessagesAreMonitored()
        {
            // Act
            _redditHelper.MonitorUnreadMessages();

            // Assert
            A.CallTo(() => _fakeRedditClientWrapper.MonitorUnread()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenRedditHelperIsMonitoringUnreadMessages_WhenNewUnreadMessageArrives_ThenMessagesHandlerMethodIsExecuted()
        {
            // Arrange
            _redditHelper.MonitorUnreadMessages();

            // Act
            _fakeRedditClientWrapper.UnreadUpdated += Raise.With(e: new MessagesUpdateEventArgs());

            // Assert
            A.CallTo(() => _fakeRedditMessageHandler.OnUnreadMessagesUpdated(A<object>._, A<MessagesUpdateEventArgs>._))
             .MustHaveHappenedOnceExactly();
        }
    }
}
