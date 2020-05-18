using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IGfyCatApi
    {
        [Post("/v1/oauth/token")]
        Task<GfyCatTokenResponse> GetAuthToken([Body] GfyCatCredentials gfyCatCredentials);
    }

    public class GfyCatCredentials
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }

    public class GfyCatTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
