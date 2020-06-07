using FakeItEasy;
using FluentAssertions;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Exceptions;
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

        private const string FakeGfyNameDuplicate = "fake_gfyNameDuplicate";

        private readonly IGfyCatApi _fakeGfyCatApi;

        private readonly IGfyCatFileDropApi _fakeGfyCatFileDropApi;

        private readonly IGfyCatApiConfiguration _fakeGfyCatApiConfiguration;

        private readonly IVideoUploader _gfyCatVideoUploader;

        public GfyCatVideoUploaderTests()
        {
            DeleteTestFiles();
            _fakeGfyCatApi = A.Fake<IGfyCatApi>();
            _fakeGfyCatFileDropApi = A.Fake<IGfyCatFileDropApi>();
            _fakeGfyCatApiConfiguration = A.Fake<IGfyCatApiConfiguration>();
            A.CallTo(() => _fakeGfyCatApiConfiguration.GetUploadTimeoutInMs()).Returns(500);
            A.CallTo(() => _fakeGfyCatApiConfiguration.GetUploadStatusPollingPeriodInMs()).Returns(10);
            _gfyCatVideoUploader = new GfyCatVideoUploader(_fakeGfyCatApi, _fakeGfyCatFileDropApi, _fakeGfyCatApiConfiguration);
        }

        public void Dispose()
        {
            DeleteTestFiles();
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalled_ThenVideoFileIsUploadedAndUrlIsReturned()
        {
            // Arrange
            CreateRotatedVideoFile();
            SetupSuccessfulApiCallStubs();

            // Act
            var gfyCatName = await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(FakeGfyName, A<StreamPart>._)).MustHaveHappenedOnceExactly();
            Assert.Equal($"https://giant.gfycat.com/{FakeGfyName}.mp4", gfyCatName);
        }

        [Fact]
        public async Task GivenRotatedVideoExistsAndVideoHasBeenPreviouslyUploaded_WhenVideoUploadIsCalled_ThenVideoFileIsUploadedAndPreviouslyUploadedVideoUrlIsReturned()
        {
            // Arrange
            CreateRotatedVideoFile(); 
            SetupSuccessfulDuplicateApiCallStubs();

             // Act
             var gfyCatName = await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(FakeGfyName, A<StreamPart>._)).MustHaveHappenedOnceExactly();
            Assert.Equal($"https://giant.gfycat.com/{FakeGfyNameDuplicate}.mp4", gfyCatName);
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalledButGfyCreationFails_ThenVideoUploadExceptionIsThrown()
        {
            // Arrange
            CreateRotatedVideoFile();
            SetupSuccessfulTokenRequestStub();
            SetupUnsuccessfulGfyCreation();

            // Act
            Func<Task> uploadAction = async () => { await _gfyCatVideoUploader.UploadAsync(); };

            // Assert
            await uploadAction.Should().ThrowAsync<VideoUploadException>();
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalledButGfyUploadTimesOut_ThenVideoUploadTimedOutExceptionIsThrown()
        {
            // Arrange
            CreateRotatedVideoFile();
            SetupSuccessfulTokenRequestStub();
            SetupSuccessfulGfyCreation();
            SetupSuccessfulEncodingGfyStatus();

            // Act
            Func<Task> uploadAction = async () => { await _gfyCatVideoUploader.UploadAsync(); };

            // Assert
            await uploadAction.Should().ThrowAsync<VideoUploadTimeOutException>();
        }

        private void SetupSuccessfulApiCallStubs()
        {
            SetupSuccessfulTokenRequestStub();
            SetupSuccessfulGfyCreation();
            SetupSuccessfulCompleteGfyStatus();
            SetupSuccessfulCompleteGetGfy();
        }

        private void SetupSuccessfulDuplicateApiCallStubs()
        {
            SetupSuccessfulTokenRequestStub();
            SetupSuccessfulGfyCreation();
            SetupSuccessfulDuplicateGfyStatus();
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

        private void SetupSuccessfulCompleteGfyStatus()
        {
            A.CallTo(() => _fakeGfyCatApi.GetGfyStatus(FakeGfyName))
                .Returns(new GfyStatusResponse
                {
                    Task = "complete"
                });
        }

        private void SetupSuccessfulEncodingGfyStatus()
        {
            A.CallTo(() => _fakeGfyCatApi.GetGfyStatus(FakeGfyName))
                .Returns(new GfyStatusResponse
                {
                    Task = "encoding"
                });
        }

        private void SetupSuccessfulDuplicateGfyStatus()
        {
            A.CallTo(() => _fakeGfyCatApi.GetGfyStatus(FakeGfyName))
                .Returns(new GfyStatusResponse
                {
                    Task = "complete",
                    Md5Found = 1,
                    Mp4Url = $"https://giant.gfycat.com/{FakeGfyNameDuplicate}.mp4"
                });
        }

        private void SetupSuccessfulCompleteGetGfy()
        {
            A.CallTo(() => _fakeGfyCatApi.GetGfy(FakeGfyName))
                .Returns(new GfyResponse
                {
                    GfyItem = new GfyItem
                    {
                        Mp4Url = $"https://giant.gfycat.com/{FakeGfyName}.mp4"
                    }
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

        private static void DeleteTestFiles()
        {
            if (File.Exists("video_rotated.mp4")) File.Delete("video_rotated.mp4");
            if (File.Exists(FakeGfyName)) File.Delete(FakeGfyName);
        }
    }
}
