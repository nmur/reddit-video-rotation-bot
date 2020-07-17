using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RedditVideoRotationBot.Media
{
    [ExcludeFromCodeCoverage]
    public class VideoDownloader : IVideoDownloader
    {
        public void DownloadFromUrl(string url)
        {
            try
            {
                using var client = new WebClient();
                client.DownloadFile(url, "video.mp4");
            }
            catch (Exception ex)
            {
                throw new VideoDownloadException(ex.Message);
            }
        }
    }
}
