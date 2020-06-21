using Xunit;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using System;
using RedditVideoRotationBot;

namespace RedditVideoRotationBotTests
{
    public class FfmpegRotationArgumentDeterminerTests
    {
        [Fact]
        public void GivenNoMessageArgument_WhenFfmpegRotationArgumentIsDetermined_ThenArgumentExceptionIsThrown()
        {
            // Arrange + Act + Assert
            Invoking(() => FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(""))
                .Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("90")]
        [InlineData("ccw")]
        [InlineData("counterclockwise")]
        [InlineData("left")]
        public void GivenMessageArgumentEquivalentTo90_WhenFfmpegRotationArgumentIsDetermined_ThenFfmpegArgumentOf90IsReturned(string messageArg)
        {
            // Arrange 
            var expectedRotationArg = "90";

            // Act
            var rotationArg = FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(messageArg);

            // Assert
            Assert.Equal(expectedRotationArg, rotationArg);
        }
    }
}
