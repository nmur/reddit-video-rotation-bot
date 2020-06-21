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
        public void Rotate(string rotationArgument)
        {
            if (File.Exists("video.mp4"))
            {
                var rotationValue = GetRotationValue();

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-i video.mp4 -c copy -metadata:s:v:0 rotate={rotationValue} video_rotated.mp4",
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

        private static string GetRotationValue()
        {
            return "90";
        }
    }
}
