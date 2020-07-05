using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage]
    public class MediaProcessor : IMediaProcessor
    {
        public void CombineVideoAndAudio()
        {
            if (File.Exists("video.mp4") && File.Exists("audio.mp4"))
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = "-i video.mp4 -i audio.mp4 -c copy video.mp4",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                proc.WaitForExit();
            }
        }
    }
}
