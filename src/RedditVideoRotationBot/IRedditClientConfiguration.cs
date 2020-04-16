namespace RedditVideoRotationBot
{
    public interface IRedditClientConfiguration
    {
        string GetAppId();

        string GetAppSecret();

        string GetRefreshToken();
    }
}