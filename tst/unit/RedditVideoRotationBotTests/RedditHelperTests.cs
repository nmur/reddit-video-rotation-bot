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
    }
}
