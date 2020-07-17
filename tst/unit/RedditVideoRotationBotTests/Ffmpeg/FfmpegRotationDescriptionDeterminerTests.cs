using Xunit;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using System;
using RedditVideoRotationBot.Interfaces;
using RedditVideoRotationBot.Media.Ffmpeg;

namespace RedditVideoRotationBotTests.Ffmpeg
{
    public class FfmpegRotationDescriptionDeterminerTests
    {
        readonly IRotationDescriptionDeterminer _ffmpegRotationDescriptionDeterminer;

        public FfmpegRotationDescriptionDeterminerTests()
        {
            _ffmpegRotationDescriptionDeterminer = new FfmpegRotationDescriptionDeterminer();
        }

        [Fact]
        public void GivenNoMessageArgument_WhenFfmpegRotationDescriptionIsDetermined_ThenArgumentExceptionIsThrown()
        {
            // Arrange + Act + Assert
            Invoking(() => FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(""))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenClockwiseMessageArgument_WhenFfmpegRotationDescriptionIsDetermined_ThenCorrectRotationDescriptionIsReturned()
        {
            // Arrange 
            const string expectedRotationDescription = "90° clockwise";

            // Act
            var rotationArg = _ffmpegRotationDescriptionDeterminer.GenerateRotationDescriptionMessageArgArgument("270");

            // Assert
            Assert.Equal(expectedRotationDescription, rotationArg);
        }

        [Fact]
        public void GivenCounterClockwiseMessageArgument_WhenFfmpegRotationDescriptionIsDetermined_ThenCorrectRotationDescriptionIsReturned()
        {
            // Arrange 
            const string expectedRotationDescription = "90° counter-clockwise";

            // Act
            var rotationArg = _ffmpegRotationDescriptionDeterminer.GenerateRotationDescriptionMessageArgArgument("90");

            // Assert
            Assert.Equal(expectedRotationDescription, rotationArg);
        }

        [Fact]
        public void Given180MessageArgument_WhenFfmpegRotationDescriptionIsDetermined_ThenCorrectRotationDescriptionIsReturned()
        {
            // Arrange 
            const string expectedRotationDescription = "180°";

            // Act
            var rotationArg = _ffmpegRotationDescriptionDeterminer.GenerateRotationDescriptionMessageArgArgument("180");

            // Assert
            Assert.Equal(expectedRotationDescription, rotationArg);
        }
    }
}
