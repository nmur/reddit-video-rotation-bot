using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;

namespace RedditVideoRotationBot
{
    public class FfmpegMediaProcessor : IMediaProcessor
    {
        private readonly IFfmpegExecutor _ffmpegExecutor;

        public FfmpegMediaProcessor(IFfmpegExecutor ffmpegExecutor)
        {
            _ffmpegExecutor = ffmpegExecutor;
        }

        public void CombineVideoAndAudio()
        {
            try
            {
                if (File.Exists("video.mp4") && File.Exists("audio.mp4"))
                {
                    _ffmpegExecutor.ExecuteFfmpegCommandWithArgString("-i video.mp4 -i audio.mp4 -c copy video.mp4");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to combine video and audio files, proceeding regardless...");
            }
        }
    }
}
