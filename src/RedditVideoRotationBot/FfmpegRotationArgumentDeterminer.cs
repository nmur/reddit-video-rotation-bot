using System;

namespace RedditVideoRotationBot
{
    public static class FfmpegRotationArgumentDeterminer
    {
        public static string GetRotationArgFromMessageArg(string messageArg)
        {
            switch (messageArg)
            {
                case "90":
                    return "90";
                default: 
                    throw new ArgumentException("Invalid or empty rotation argument from message.");
            }
        }
    }
}
