using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class RedditClientConfiguration : IRedditClientConfiguration
    {
        private readonly string AppId;

        private readonly string AppSecret;

        private readonly string RefreshToken;

        public RedditClientConfiguration(string appId, string appSecret, string refreshToken)
        {
            AppId = appId;
            AppSecret = appSecret;
            RefreshToken = refreshToken;
        }

        public string GetAppId()
        {
            return AppId;
        }

        public string GetAppSecret()
        {
            return AppSecret;
        }

        public string GetRefreshToken()
        {
            return RefreshToken;
        }
    }
}