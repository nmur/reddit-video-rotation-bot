using RedditVideoRotationBot;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var worker = new Worker();

            // Act
            var result = worker.IncrementInt(1);

            // Assert
            Assert.Equal(2, result);
        }
    }
}
