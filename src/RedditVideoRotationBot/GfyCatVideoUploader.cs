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

        private readonly IGfyCatApiConfiguration _gfyCatApiConfiguration;

        public GfyCatVideoUploader(IGfyCatApi gfyCatApi, IGfyCatFileDropApi gfyCatFileDropApi, IGfyCatApiConfiguration gfyCatApiConfiguration)
        {
            _gfyCatApi = gfyCatApi;
            _gfyCatFileDropApi = gfyCatFileDropApi;
            _gfyCatApiConfiguration = gfyCatApiConfiguration;
        }

        public async Task UploadAsync()
        {
            string token = await GetAuthToken();
            var gfyCreationResponse = await _gfyCatApi.CreateGfy($"Bearer {token}");

            if (gfyCreationResponse.IsOk && File.Exists("video_rotated.mp4"))
            {
                await UploadVideo(gfyCreationResponse.GfyName);
            }
        }

        private async Task<string> GetAuthToken()
        {
            var gfyCatTokenResponse = await _gfyCatApi.GetAuthToken(new GfyCatCredentials
            {
                GrantType = "client_credentials",
                ClientId = _gfyCatApiConfiguration.GetClientId(),
                ClientSecret = _gfyCatApiConfiguration.GetClientSecret()
            });
            return gfyCatTokenResponse.AccessToken;
        }

        private async Task UploadVideo(string gfyName)
        {
            Console.WriteLine($"Gfyname: {gfyName}");
            File.Move("video_rotated.mp4", gfyName);
            using var stream = File.OpenRead(gfyName);

            await _gfyCatFileDropApi.UploadVideoFromFile(gfyName, new StreamPart(stream, gfyName));

            var task = WaitForVideoUploadToComplete(gfyName);
            if (await Task.WhenAny(task, Task.Delay(_gfyCatApiConfiguration.GetUploadTimeoutInMs())) == task)
            {
                string mp4Url;

                var gfyStatusResponse = await _gfyCatApi.GetGfyStatus(gfyName);
                if (gfyStatusResponse.Md5Found == 1)
                {
                    mp4Url = gfyStatusResponse.Mp4Url;
                }
                else
                {
                    var gfyResponse = await _gfyCatApi.GetGfy(gfyName);
                    mp4Url = gfyResponse.GfyItem.Mp4Url;
                }

                Console.WriteLine($"Reuploaded video URL: {mp4Url}");
            }
            else
            {
                Console.WriteLine($"Timed-out while waiting for video upload and encode to complete");
            }
        }

        private async Task WaitForVideoUploadToComplete(string gfyName)
        {
            var status = "";
            while (status != "complete")
            {
                Thread.Sleep(5000);
                var gfyStatusResponse = await _gfyCatApi.GetGfyStatus(gfyName);
                status = gfyStatusResponse.Task;
                Console.WriteLine($"Current status of video: {status}");
            }
            return;
        }
    }
}
