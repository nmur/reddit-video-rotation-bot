using RedditVideoRotationBot.Interfaces;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage] //TODO: should be integration tested though
    public class VideoRotator : IVideoRotator
    {
        public void Rotate()
        {
            if (File.Exists("video.mp4"))
            {
                Console.WriteLine($"video.mp4 found");
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = "-i video.mp4 -c copy -metadata:s:v:0 rotate=90 video_rotated.mp4",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                Console.WriteLine($"Starting video rotation...");
                proc.Start();
                Console.WriteLine($"Started video rotation, waiting for process to complete...");
                proc.WaitForExit();
                Console.WriteLine($"Process complete with exit code: {proc.ExitCode}");
            }

            if (File.Exists("video_rotated.mp4"))
            {
                Console.WriteLine($"video rotated successfully!");
            }
            else
            {
                Console.WriteLine($"ERROR: video rotated unsuccessfully!");
            }
        }
    }
}
