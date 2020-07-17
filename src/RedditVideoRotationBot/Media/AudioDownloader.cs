using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RedditVideoRotationBot.Media
{
    [ExcludeFromCodeCoverage]
    public class AudioDownloader : IAudioDownloader
    {
        public void DownloadFromUrl(string url)
        {
            try
            {
                using var client = new WebClient();
                client.DownloadFile(url, "audio.mp4");
            }
            catch (Exception)
            {
                Console.WriteLine("No audio available for video, proceeding regardless...");
            }
        }
    }
}
