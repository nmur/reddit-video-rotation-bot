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

        [Fact]
        public void GivenMessageArgumentOf90_WhenFfmpegRotationArgumentIsDetermined_ThenFfmpegArgumentOf90IsReturned()
        {
            // Arrange 
            var expectedRotationArg = "90";

            // Act
            var rotationArg = FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg("90");

            // Assert
            Assert.Equal(expectedRotationArg, rotationArg);
        }
    }
}
