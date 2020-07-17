using FakeItEasy;
using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using static FluentAssertions.FluentActions;
using RedditVideoRotationBot.Media;

namespace RedditVideoRotationBotTests.Media
{
    public class MediaProcessorTests
    {
        private const string VideoUrlString = "https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback";

        private const string AudioUrlString = "https://v.redd.it/abcabcabcabc/audio";

        private const string RotationArgumentString = "cw";

        private const string UploadUrl = "https://giant.gfycat.com/SomeVideoUrl.mp4";

        private readonly IVideoDownloader _fakeVideoDownloader;

        private readonly IAudioDownloader _fakeAudioDownloader;

        private readonly IMediaMuxer _fakeMediaMuxer;

        private readonly IVideoRotator _fakeVideoRotator;

        private readonly IVideoUploader _fakeGfyCatVideoUploader;

        private readonly IMediaProcessor _mediaProcessor;

        public MediaProcessorTests()
        {
            _fakeVideoDownloader = A.Fake<IVideoDownloader>();
            _fakeAudioDownloader = A.Fake<IAudioDownloader>();
            _fakeMediaMuxer = A.Fake<IMediaMuxer>();
            _fakeVideoRotator = A.Fake<IVideoRotator>();
            _fakeGfyCatVideoUploader = A.Fake<IVideoUploader>();
            _mediaProcessor = new MediaProcessor(_fakeVideoDownloader, _fakeAudioDownloader, _fakeMediaMuxer, _fakeVideoRotator, _fakeGfyCatVideoUploader);
        }

        [Fact]
        public async Task GivenValidMediaProcessorParameters_WhenAllOperationsSucceed_ThenUrlOfUploadedVideoIsReturned()
        {
            // Arrange
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).Returns(UploadUrl);

            // Act
            var uploadUrl = await _mediaProcessor.DownloadAndRotateAndUploadVideo(GetValidMediaProcessorParameters());

            // Assert
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeMediaMuxer.CombineVideoAndAudio()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeVideoRotator.Rotate(RotationArgumentString)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(UploadUrl, uploadUrl);
        }

        [Fact]
        public void GivenValidMediaProcessorParameters_WhenVideoDownloadFails_ThenMediaProcessingStops()
        {
            // Arrange
            A.CallTo(() => _fakeVideoDownloader.DownloadFromUrl(VideoUrlString)).Throws<VideoDownloadException>();

            // Act
            Invoking(() => _mediaProcessor.DownloadAndRotateAndUploadVideo(GetValidMediaProcessorParameters())).Should().Throw<MediaProcessorException>();

            // Assert
            A.CallTo(() => _fakeAudioDownloader.DownloadFromUrl(AudioUrlString)).MustNotHaveHappened();
            A.CallTo(() => _fakeMediaMuxer.CombineVideoAndAudio()).MustNotHaveHappened();
            A.CallTo(() => _fakeVideoRotator.Rotate(RotationArgumentString)).MustNotHaveHappened();
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public void GivenValidMediaProcessorParameters_WhenVideoRotateFails_ThenMediaProcessingStops()
        {
            // Arrange
            A.CallTo(() => _fakeVideoRotator.Rotate(A<string>._)).Throws<VideoRotateException>();

            // Act
            Invoking(() => _mediaProcessor.DownloadAndRotateAndUploadVideo(GetValidMediaProcessorParameters())).Should().Throw<MediaProcessorException>();

            // Assert
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public void GivenValidMediaProcessorParameters_WhenVideoUploadFails_ThenMediaProcessingStops()
        {
            // Arrange
            A.CallTo(() => _fakeGfyCatVideoUploader.UploadAsync()).Throws<VideoUploadException>();

            // Act + Assert
            Invoking(() => _mediaProcessor.DownloadAndRotateAndUploadVideo(GetValidMediaProcessorParameters())).Should().Throw<MediaProcessorException>();
        }

        private MediaProcessorParameters GetValidMediaProcessorParameters()
        {
            return new MediaProcessorParameters
            {
                AudioUrl = AudioUrlString,
                VideoUrl = VideoUrlString,
                RotationArgument = RotationArgumentString
            };
        }
    }
}
