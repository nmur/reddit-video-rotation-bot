using RedditVideoRotationBot.Interfaces;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Media.Ffmpeg
{

    [ExcludeFromCodeCoverage]
    public class FfmpegExecutor : IFfmpegExecutor
    {
        public void ExecuteFfmpegCommandWithArgString(string argString)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = argString,
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
