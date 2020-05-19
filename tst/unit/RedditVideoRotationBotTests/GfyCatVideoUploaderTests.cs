using FakeItEasy;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Interfaces;
using Refit;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RedditVideoRotationBotTests
{
    public class GfyCatVideoUploaderTests
    {
        private readonly IGfyCatApi _fakeGfyCatApi;

        private readonly IGfyCatFileDropApi _fakeGfyCatFileDropApi;

        private readonly IVideoUploader _gfyCatVideoUploader;

        public GfyCatVideoUploaderTests()
        {
            _fakeGfyCatApi = A.Fake<IGfyCatApi>();
            _fakeGfyCatFileDropApi = A.Fake<IGfyCatFileDropApi>();
            _gfyCatVideoUploader = new GfyCatVideoUploader(_fakeGfyCatApi, _fakeGfyCatFileDropApi);
        }

        [Fact]
        public async Task GivenRotatedVideoExists_WhenVideoUploadIsCalled_ThenVideoFileIsUploaded()
        {
            // Arrange
            CreateRotatedVideoFile();

            A.CallTo(() => _fakeGfyCatApi.GetAuthToken(A<GfyCatCredentials>._))
                .Returns(new GfyCatTokenResponse
                {
                    AccessToken = "fake_token"
                });

            A.CallTo(() => _fakeGfyCatApi.CreateGfy(A<string>._))
                .Returns(new GfyCreationResponse
                {
                    IsOk = true,
                    GfyName = "fake_gfyName"
                });

            // Act
            await _gfyCatVideoUploader.UploadAsync();

            // Assert
            A.CallTo(() => _fakeGfyCatFileDropApi.UploadVideoFromFile(A<string>._, A<StreamPart>._)).MustHaveHappenedOnceExactly();
        }

        private static void CreateRotatedVideoFile()
        {
            using var fs = File.Create("video_rotated.mp4");
            var info = new UTF8Encoding(true).GetBytes("Adding some text into the file.");
            fs.Write(info, 0, info.Length);
        }
    }
}
