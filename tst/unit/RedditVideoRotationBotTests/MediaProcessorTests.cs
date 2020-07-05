using Xunit;
using RedditVideoRotationBot;
using FakeItEasy;
using RedditVideoRotationBot.Interfaces;
using System.IO;
using System;
using System.Text;
using static FluentAssertions.FluentActions;
using FluentAssertions;

namespace RedditVideoRotationBotTests
{
    public class MediaProcessorTests : IDisposable
    {
        private readonly IFfmpegExecutor _fakeFfmpegExecutor;

        private readonly IMediaProcessor _mediaProcessor;

        public MediaProcessorTests()
        {
            DeleteTestFiles();
            _fakeFfmpegExecutor = A.Fake<IFfmpegExecutor>();
            _mediaProcessor = new MediaProcessor(_fakeFfmpegExecutor);
        }

        public void Dispose()
        {
            DeleteTestFiles();
        }

        [Fact]
        public void GivenVideoAndAudioFilesAreFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsCalled()
        {
            // Arrange
            CreateVideoFile();
            CreateAudioFile();

            // Act
            _mediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenVideoFileFoundButAudioFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateVideoFile();

            // Act
            _mediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenAudioFileFoundButVideoFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateAudioFile();

            // Act
            _mediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenVideoAndAudioFilesAreFound_WhenVideoAndAudioFilesCominationIsRequestedAndFfmpegCommandThrowException_ThenNoExceptionIsPropagated()
        {
            // Arrange
            CreateVideoFile();
            CreateAudioFile();
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).Throws<Exception>();

            // Act + Assert
            Invoking(() => _mediaProcessor.CombineVideoAndAudio()).Should().NotThrow();
        }

        private static void CreateVideoFile()
        {
            if (!File.Exists("video.mp4"))
            {
                using var fs = File.Create("video.mp4");
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }

        private static void CreateAudioFile()
        {
            if (!File.Exists("audio.mp4"))
            {
                using var fs = File.Create("audio.mp4");
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }

        private static void DeleteTestFiles()
        {
            if (File.Exists("video.mp4")) File.Delete("video.mp4");
            if (File.Exists("audio.mp4")) File.Delete("audio.mp4");
        }
    }
}
