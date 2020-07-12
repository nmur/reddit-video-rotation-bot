using FakeItEasy;
using RedditVideoRotationBot.Interfaces;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class MediaProcessorTests
    {
        private readonly IVideoDownloader _fakeVideoDownloader;

        private readonly IAudioDownloader _fakeAudioDownloader;

        private readonly IMediaMuxer _fakeMediaMuxer;

        private readonly IVideoRotator _fakeVideoRotator;

        private readonly IVideoUploader _fakeGfyCatVideoUploader;

        public MediaProcessorTests()
        {
            _fakeVideoDownloader = A.Fake<IVideoDownloader>();
            _fakeAudioDownloader = A.Fake<IAudioDownloader>();
            _fakeMediaMuxer = A.Fake<IMediaMuxer>();
            _fakeVideoRotator = A.Fake<IVideoRotator>();
            _fakeGfyCatVideoUploader = A.Fake<IVideoUploader>();
        }
    }
}
