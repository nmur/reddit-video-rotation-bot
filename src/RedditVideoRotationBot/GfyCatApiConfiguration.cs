using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class GfyCatApiConfiguration : IGfyCatApiConfiguration
    {
        private readonly string ClientId;

        private readonly string ClientSecret;

        public GfyCatApiConfiguration(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string GetClientId()
        {
            return ClientId;
        }

        public string GetClientSecret()
        {
            return ClientSecret;
        }
    }
}