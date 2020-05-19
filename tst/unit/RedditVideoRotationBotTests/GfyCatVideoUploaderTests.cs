using FakeItEasy;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Interfaces;
using Refit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)] //until we get unique video file names

namespace RedditVideoRotationBotTests
{
    public class GfyCatVideoUploaderTests : IDisposable
    {
        private const string FakeToken = "fake_token";

        private const string FakeGfyName = "fake_gfyName";

        private readonly IGfyCatApi _fakeGfyCatApi;

        private readonly IGfyCatFileDropApi _fakeGfyCatFileDropApi;

        private readonly IVideoUploader _gfyCatVideoUploader;

        public GfyCatVideoUploaderTests()
        {
            _fakeGfyCatApi = A.Fake<IGfyCatApi>();
            _fakeGfyCatFileDropApi = A.Fake<IGfyCatFileDropApi>();
            _gfyCatVideoUploader = new GfyCatVideoUploader(_fakeGfyCatApi, _fakeGfyCatFileDropApi);
        }

        public void Dispose()
        {
            if (File.Exists("video_rotated.mp4")) File.Delete("video_rotated.mp4");
            if (File.Exists(FakeGfyName)) File.Delete(FakeGfyName);
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalled_ThenVideoFileIsUploaded()
        {
            // Arrange
            CreateRotatedVideoFile();
            SetupSuccessfulApiCallStubs();

            // Act
            await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(FakeGfyName, A<StreamPart>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalledButGfyCreationFails_ThenVideoFileIsNotUploaded()
        {
            // Arrange
            CreateRotatedVideoFile();
            SetupSuccessfulTokenRequestStub();
            SetupUnsuccessfulGfyCreation();

            // Act
            await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(FakeGfyName, A<StreamPart>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GivenRotatedVideoDoesNotExist_WhenVideoUploadIsCalled_ThenVideoFileIsNotUploaded()
        {
            // Arrange
            SetupSuccessfulApiCallStubs();

            // Act
            await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(FakeGfyName, A<StreamPart>._)).MustNotHaveHappened();
        }

        private void SetupSuccessfulApiCallStubs()
        {
            SetupSuccessfulTokenRequestStub();
            SetupSuccessfulGfyCreation();
        }

        private void SetupSuccessfulTokenRequestStub()
        {
            A.CallTo(() => _fakeGfyCatApi.GetAuthToken(A<GfyCatCredentials>._))
                .Returns(new GfyCatTokenResponse
                {
                    AccessToken = FakeToken
                });
        }

        private void SetupSuccessfulGfyCreation()
        {
            A.CallTo(() => _fakeGfyCatApi.CreateGfy($"Bearer {FakeToken}"))
                .Returns(new GfyCreationResponse
                {
                    IsOk = true,
                    GfyName = FakeGfyName
                });
        }

        private void SetupUnsuccessfulGfyCreation()
        {
            A.CallTo(() => _fakeGfyCatApi.CreateGfy($"Bearer {FakeToken}"))
                .Returns(new GfyCreationResponse
                {
                    IsOk = false
                });
        }

        private static void CreateRotatedVideoFile()
        {
            if (!File.Exists("video_rotated.mp4"))
            {
                using var fs = File.Create("video_rotated.mp4");
                var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
