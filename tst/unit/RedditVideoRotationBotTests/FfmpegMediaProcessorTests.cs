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
    public class FfmpegMediaProcessorTests : IDisposable
    {
        private const string VideoFileName = "video.mp4";

        private const string AudioFileName = "audio.mp4";

        private readonly IFfmpegExecutor _fakeFfmpegExecutor;

        private readonly IMediaProcessor _FfmpegMediaProcessor;

        public FfmpegMediaProcessorTests()
        {
            DeleteTestFiles();
            _fakeFfmpegExecutor = A.Fake<IFfmpegExecutor>();
            _FfmpegMediaProcessor = new FfmpegMediaProcessor(_fakeFfmpegExecutor);
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
            _FfmpegMediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenVideoFileFoundButAudioFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateVideoFile();

            // Act
            _FfmpegMediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenAudioFileFoundButVideoFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateAudioFile();

            // Act
            _FfmpegMediaProcessor.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenVideoAndAudioFilesAreFound_WhenVideoAndAudioFilesCominationIsRequestedAndFfmpegCommandThrowsException_ThenNoExceptionIsPropagated()
        {
            // Arrange
            CreateVideoFile();
            CreateAudioFile();
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).Throws<Exception>();

            // Act + Assert
            Invoking(() => _FfmpegMediaProcessor.CombineVideoAndAudio()).Should().NotThrow();
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

        private static void CreateAudioFile()
        {
            if (!File.Exists(AudioFileName))
            {
                using var fs = File.Create(AudioFileName);
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }

        private static void DeleteTestFiles()
        {
            if (File.Exists(VideoFileName)) File.Delete(VideoFileName);
            if (File.Exists(AudioFileName)) File.Delete(AudioFileName);
        }
    }
}
