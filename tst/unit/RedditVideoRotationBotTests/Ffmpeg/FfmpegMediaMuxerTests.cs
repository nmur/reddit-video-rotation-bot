using Xunit;
using FakeItEasy;
using RedditVideoRotationBot.Interfaces;
using System.IO;
using System;
using System.Text;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using RedditVideoRotationBot.Media.Ffmpeg;

namespace RedditVideoRotationBotTests.Ffmpeg
{
    public class FfmpegMediaMuxerTests : IDisposable
    {
        private const string VideoFileName = "video.mp4";

        private const string AudioFileName = "audio.mp4";

        private readonly IFfmpegExecutor _fakeFfmpegExecutor;

        private readonly IMediaMuxer _FfmpegMediaMuxer;

        public FfmpegMediaMuxerTests()
        {
            DeleteTestFiles();
            _fakeFfmpegExecutor = A.Fake<IFfmpegExecutor>();
            _FfmpegMediaMuxer = new FfmpegMediaMuxer(_fakeFfmpegExecutor);
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
            _FfmpegMediaMuxer.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GivenVideoFileFoundButAudioFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateVideoFile();

            // Act
            _FfmpegMediaMuxer.CombineVideoAndAudio();

            // Assert
            A.CallTo(() => _fakeFfmpegExecutor.ExecuteFfmpegCommandWithArgString(A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GivenAudioFileFoundButVideoFileNotFound_WhenVideoAndAudioFilesCominationIsRequested_ThenFfmpegCommandWithArgsIsNotCalled()
        {
            // Arrange
            CreateAudioFile();

            // Act
            _FfmpegMediaMuxer.CombineVideoAndAudio();

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
            Invoking(() => _FfmpegMediaMuxer.CombineVideoAndAudio()).Should().NotThrow();
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
