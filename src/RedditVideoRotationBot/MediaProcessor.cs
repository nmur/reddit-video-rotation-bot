using RedditVideoRotationBot.Interfaces;
using System.IO;

namespace RedditVideoRotationBot
{
    public class MediaProcessor : IMediaProcessor
    {
        private readonly IFfmpegExecutor _ffmpegExecutor;

        public MediaProcessor(IFfmpegExecutor ffmpegExecutor)
        {
            _ffmpegExecutor = ffmpegExecutor;
        }

        public void CombineVideoAndAudio()
        {
            if (File.Exists("video.mp4") && File.Exists("audio.mp4"))
            {
                _ffmpegExecutor.ExecuteFfmpegCommandWithArgString("-i video.mp4 -i audio.mp4 -c copy video.mp4");
            }
        }
    }
}
