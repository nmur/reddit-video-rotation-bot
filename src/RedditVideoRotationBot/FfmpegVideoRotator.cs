using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage] //TODO: should be integration tested though
    public class FfmpegVideoRotator : IVideoRotator
    {
        public void Rotate(string messageArg)
        {
            if (File.Exists("video.mp4"))
            {
                var rotationArg = FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(messageArg);

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-i video.mp4 -c copy -metadata:s:v:0 rotate={rotationArg} video_rotated.mp4",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                proc.WaitForExit();
            }

            if (File.Exists("video_rotated.mp4"))
            {
                Console.WriteLine($"video rotated successfully!");
            }
            else
            {
                throw new VideoRotateException("Video rotation failed.");
            }
        }
    }
}
