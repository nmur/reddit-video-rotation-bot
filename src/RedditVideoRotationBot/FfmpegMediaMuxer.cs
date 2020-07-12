using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;

namespace RedditVideoRotationBot
{
    public class FfmpegMediaMuxer : IMediaMuxer
    {
        private readonly IFfmpegExecutor _ffmpegExecutor;

        public FfmpegMediaMuxer(IFfmpegExecutor ffmpegExecutor)
        {
            _ffmpegExecutor = ffmpegExecutor;
        }

        public void CombineVideoAndAudio()
        {
            try
            {
                if (File.Exists("video.mp4") && File.Exists("audio.mp4"))
                {
                    _ffmpegExecutor.ExecuteFfmpegCommandWithArgString("-y -i video.mp4 -i audio.mp4 -c copy combined_video.mp4");
                    File.Delete("video.mp4");
                    File.Move("combined_video.mp4", "video.mp4");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to combine video and audio files, proceeding regardless...");
            }
        }
    }
}
