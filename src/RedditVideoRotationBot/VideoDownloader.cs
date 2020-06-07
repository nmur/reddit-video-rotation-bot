using RedditVideoRotationBot.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage]
    public class VideoDownloader : IVideoDownloader
    {
        public void DownloadFromUrl(string url)
        {
            using var client = new WebClient();
            client.DownloadFile(url, "video.mp4");
        }
    }
}
