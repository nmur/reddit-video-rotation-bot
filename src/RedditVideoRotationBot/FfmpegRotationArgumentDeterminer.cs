﻿using System;

namespace RedditVideoRotationBot
{
    public static class FfmpegRotationArgumentDeterminer
    {
        public static string GetRotationArgFromMessageArg(string messageArg)
        {
            switch (messageArg.ToLower())
            {
                case "90":
                case "ccw":
                case "counterclockwise":
                case "left":
                    return "90";
                default: 
                    throw new ArgumentException("Invalid or empty rotation argument from message.");
            }
        }
    }
}
