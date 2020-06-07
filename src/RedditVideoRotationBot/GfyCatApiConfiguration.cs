using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class GfyCatApiConfiguration : IGfyCatApiConfiguration
    {
        private readonly string ClientId;

        private readonly string ClientSecret;

        private readonly int UploadTimeoutInMs;

        public GfyCatApiConfiguration(string clientId, string clientSecret, int uploadTimeoutInMs)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            UploadTimeoutInMs = uploadTimeoutInMs;
        }

        public string GetClientId()
        {
            return ClientId;
        }

        public string GetClientSecret()
        {
            return ClientSecret;
        }

        public int GetUploadTimeoutInMs()
        {
            return UploadTimeoutInMs;
        }
    }
}