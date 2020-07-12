using RedditVideoRotationBot.Exceptions;
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

        public async Task<string> UploadAsync()
        {
            string token = await GetAuthToken();
            var gfyCreationResponse = await _gfyCatApi.CreateGfy($"Bearer {token}", new GfyCatCreationParameters{KeepAudio = true});

            if (gfyCreationResponse.IsOk)
            {
                return await UploadVideoAndReturnMp4Url(gfyCreationResponse.GfyName);
            }
            else
            {
                throw new VideoUploadException("Gfy creation was not successful");
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

        private async Task<string> UploadVideoAndReturnMp4Url(string gfyName)
        {
            Console.WriteLine($"Gfyname: {gfyName}");
            RenameRotatedVideoFileToGfyName(gfyName);
            await UploadVideoToFileDrop(gfyName);

            var task = GetWaitForVideoUploadToCompleteTask(gfyName);
            if (task.Wait(_gfyCatApiConfiguration.GetUploadTimeoutInMs()))
            {
                DeleteVideoFile(gfyName);
                return await GetMp4UrlForGfyName(gfyName);
            }
            else
            {
                throw new VideoUploadTimeOutException("Timed-out while waiting for video upload and encode to complete");
            }
        }

        private async Task UploadVideoToFileDrop(string gfyName)
        {
            using var stream = File.OpenRead(gfyName);
            await _gfyCatFileDropApi.UploadVideoFromFile(gfyName, new StreamPart(stream, gfyName));
        }

        private static void RenameRotatedVideoFileToGfyName(string gfyName)
        {
            File.Move("video_rotated.mp4", gfyName);
        }

        private static void DeleteVideoFile(string gfyName)
        {
            if (File.Exists(gfyName)) File.Delete(gfyName);
        }

        private async Task<string> GetMp4UrlForGfyName(string gfyName)
        {
            var gfyStatusResponse = await GetGfyStatus(gfyName);

            if (WasVideoUploadedPreviously(gfyStatusResponse))
            {
                return GetMp4UrlForPreviouslyUploadedVideo(gfyStatusResponse);
            }
            else
            {
                return await GetMp4UrlForUploadedVideo(gfyName);
            }
        }

        private async Task<GfyStatusResponse> GetGfyStatus(string gfyName)
        {
            return await _gfyCatApi.GetGfyStatus(gfyName);
        }

        private static bool WasVideoUploadedPreviously(GfyStatusResponse gfyStatusResponse)
        {
            return gfyStatusResponse.Md5Found == 1;
        }

        private static string GetMp4UrlForPreviouslyUploadedVideo(GfyStatusResponse gfyStatusResponse)
        {
            return gfyStatusResponse.Mp4Url;
        }

        private async Task<string> GetMp4UrlForUploadedVideo(string gfyName)
        {
            var gfyResponse = await GetGfy(gfyName);
            return gfyResponse.GfyItem.Mp4Url;
        }

        private async Task<GfyResponse> GetGfy(string gfyName)
        {
            return await _gfyCatApi.GetGfy(gfyName);
        }

        private async Task GetWaitForVideoUploadToCompleteTask(string gfyName)
        {
            await Task.Run(async () =>
             {
                 var status = "";
                 while (status != "complete")
                 {
                     Thread.Sleep(_gfyCatApiConfiguration.GetUploadStatusPollingPeriodInMs());
                     status = (await GetGfyStatus(gfyName)).Task;
                     Console.WriteLine($"Current status of video: {status}");
                 }
                 return;
             });
        }
    }
}
