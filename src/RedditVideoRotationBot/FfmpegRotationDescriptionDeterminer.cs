using RedditVideoRotationBot.Interfaces;
using System;

namespace RedditVideoRotationBot
{
    public class FfmpegRotationDescriptionDeterminer : IRotationDescriptionDeterminer
    {
        public string GenerateRotationDescriptionMessageArgArgument(string messageArg)
        {
            return (FfmpegRotationArgumentDeterminer.GetRotationArgFromMessageArg(messageArg)) switch
            {
                "90" => "90° counter-clockwise",
                "270" => "90° clockwise",
                "180" => "180°",
                _ => throw new ArgumentException("Invalid or empty rotation message."),
            };
        }
    }
}
