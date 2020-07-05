using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;

namespace RedditVideoRotationBot
{
    public class FfmpegVideoRotator : IVideoRotator
    {
        private readonly IFfmpegExecutor _ffmpegExecutor;

        public FfmpegVideoRotator(IFfmpegExecutor ffmpegExecutor)
        {
            _ffmpegExecutor = ffmpegExecutor;
        }

        public void Rotate(string messageArg)
        {
            if (File.Exists("video.mp4"))
            {
                var rotationArg = FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(messageArg);
                _ffmpegExecutor.ExecuteFfmpegCommandWithArgString($"-i video.mp4 -c copy -metadata:s:v:0 rotate={rotationArg} video_rotated.mp4");
            }

            if (File.Exists("video_rotated.mp4"))
            {
                Console.WriteLine("video rotated successfully!");
            }
            else
            {
                throw new VideoRotateException("Video rotation failed.");
            }
        }
    }
}
