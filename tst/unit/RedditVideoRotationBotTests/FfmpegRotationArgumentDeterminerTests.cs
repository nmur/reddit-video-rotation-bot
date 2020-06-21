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
        public void GivenNoMessageArgument_WhenFfmpegRotationArgumentIsDetermined_ThenArgumentNullExceptionIsThrown()
        {
            // Act + Assert
            Invoking(() => FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(""))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
