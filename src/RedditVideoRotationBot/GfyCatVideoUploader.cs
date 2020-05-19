using RedditVideoRotationBot.Interfaces;
using Refit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RedditVideoRotationBot
{
    public class GfyCatVideoUploader : IVideoUploader
    {
        private readonly IGfyCatApi _gfyCatApi;

        private readonly IGfyCatFileDropApi _gfyCatFileDropApi;

        public GfyCatVideoUploader(IGfyCatApi gfyCatApi, IGfyCatFileDropApi gfyCatFileDropApi)
        {
            _gfyCatApi = gfyCatApi;
            _gfyCatFileDropApi = gfyCatFileDropApi;
        }

        public async Task UploadAsync()
        {
            var gfyCatTokenResponse = await _gfyCatApi.GetAuthToken(new GfyCatCredentials
            {
                GrantType = "client_credentials",
                ClientId = "",
                ClientSecret = ""
            });
            var token = gfyCatTokenResponse.AccessToken;

            var gfyCreationResponse = await _gfyCatApi.CreateGfy($"Bearer {token}");

            if (gfyCreationResponse.IsOk && File.Exists("video_rotated.mp4"))
            {
                Console.WriteLine($"Gfyname: {gfyCreationResponse.GfyName}");
                File.Move("video_rotated.mp4", gfyCreationResponse.GfyName);

                using var stream = File.OpenRead(gfyCreationResponse.GfyName);

                await _gfyCatFileDropApi.UploadVideoFromFile(gfyCreationResponse.GfyName, new StreamPart(stream, gfyCreationResponse.GfyName));
                Thread.Sleep(10000);
                var response = await _gfyCatApi.GetGfyStatus(gfyCreationResponse.GfyName);
                Console.WriteLine($"Reuploaded video URL: {response.Mp4Url}");
            }
        }
    }
}
