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

        [Theory]
        [InlineData("270")]
        [InlineData("cw")]
        [InlineData("clockwise")]
        [InlineData("right")]
        public void GivenMessageArgumentEquivalentTo270_WhenFfmpegRotationArgumentIsDetermined_ThenFfmpegArgumentOf270IsReturned(string messageArg)
        {
            // Arrange 
            var expectedRotationArg = "270";

            // Act
            var rotationArg = FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(messageArg);

            // Assert
            Assert.Equal(expectedRotationArg, rotationArg);
        }
    }
}
