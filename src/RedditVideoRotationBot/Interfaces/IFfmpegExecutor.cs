namespace RedditVideoRotationBot.Interfaces
{

    public interface IFfmpegExecutor
    {
        void ExecuteFfmpegCommandWithArgString(string argString);
    }
}
