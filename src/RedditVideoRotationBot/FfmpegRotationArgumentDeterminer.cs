using System;

namespace RedditVideoRotationBot
{
    public static class FfmpegRotationArgumentDeterminer
    {
        public static string GetRotationArgFromMessageArg(string messageArg)
        {
            switch (messageArg)
            {
                default: throw new ArgumentNullException("Invalid or empty rotation argument from message.");
            }
        }
    }
}
