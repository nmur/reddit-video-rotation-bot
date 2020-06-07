namespace RedditVideoRotationBot.Interfaces
{
    public interface IRedditClientConfiguration
    {
        string GetAppId();

        string GetAppSecret();

        string GetRefreshToken();
    }
}