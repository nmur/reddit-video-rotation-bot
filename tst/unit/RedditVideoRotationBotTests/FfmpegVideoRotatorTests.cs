using Xunit;
using RedditVideoRotationBot;
using FakeItEasy;
using RedditVideoRotationBot.Interfaces;
using System.IO;
using System.Text;
using System;
using FluentAssertions;
using static FluentAssertions.FluentActions;
using RedditVideoRotationBot.Exceptions;

namespace RedditVideoRotationBotTests
{
    public class FfmpegVideoRotatorTests : IDisposable
    {
        private const string VideoFileName = "video.mp4";

        private const string RotatedVideoFileName = "video_rotated.mp4";

        private readonly IFfmpegExecutor _fakeFfmpegExecutor;

        private readonly IVideoRotator _ffmpegVideoRotator;

        public FfmpegVideoRotatorTests()
        {
            DeleteTestFiles();
            _fakeFfmpegExecutor = A.Fake<IFfmpegExecutor>();
            _ffmpegVideoRotator = new FfmpegVideoRotator(_fakeFfmpegExecutor);
        }

        public void Dispose()
        {
            DeleteTestFiles();
        }

        [Fact]
        public void GivenVideoFileIsFound_WhenVideoRotationIsRequested_ThenFfmpegCommandWithArgsIsCalledAndVideoIsRotated()
        {
            // Arrange
            CreateVideoFile();
            CreateRotatedVideoFile();

            // Act
            Invoking(() => _ffmpegVideoRotator.Rotate("cw")).Should().NotThrow();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenVideoFileIsNotFound_WhenVideoRotationIsRequested_ThenFfmpegCommandWithArgsIsNotCalledAndVideoIsNotRotated()
        {
            // Act
            Invoking(() => _ffmpegVideoRotator.Rotate("cw")).Should().Throw<VideoRotateException>();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenVideoFileIsFound_WhenVideoRotationIsRequestedAndFfmpegCommandThrowsException_ThenVideoIsNotRotated()
        {
            // Arrange
            CreateVideoFile();

            // Act + Assert
            Invoking(() => _ffmpegVideoRotator.Rotate("cw")).Should().Throw<VideoRotateException>();
        }

        private static void CreateVideoFile()
        {
            if (!File.Exists(VideoFileName))
            {
                using var fs = File.Create(VideoFileName);
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }

        private static void CreateRotatedVideoFile()
        {
            if (!File.Exists(RotatedVideoFileName))
            {
                using var fs = File.Create(RotatedVideoFileName);
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }

        private static void DeleteTestFiles()
        {
            if (File.Exists(VideoFileName)) File.Delete(VideoFileName);
            if (File.Exists(RotatedVideoFileName)) File.Delete(RotatedVideoFileName);
        }
    }
}
