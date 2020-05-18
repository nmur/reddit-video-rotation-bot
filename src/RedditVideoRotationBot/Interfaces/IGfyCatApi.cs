﻿using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IGfyCatApi
    {
        [Post("/oauth/token")]
        Task<GfyCatTokenResponse> GetAuthToken([Body] GfyCatCredentials gfyCatCredentials);

        [Post("/gfycats")]
        Task<GfyCreationResponse> CreateGfy([Header("Authorization")] string authorization);

        [Get("/gfycats/fetch/status/{gfyName}")]
        Task<GfyStatusResponse> GetGfyStatus(string gfyName);
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

    public class GfyCreationResponse
    {
        [JsonProperty("isOk")]
        public bool IsOk { get; set; }

        [JsonProperty("gfyname")]
        public string GfyName { get; set; }
    }

    public interface IGfyCatFileDropApi
    {
        [Multipart]
        [Post("")]
        Task UploadVideoFromFile([AliasAs("key")] string key, [AliasAs("file")] StreamPart stream);
    }

    public class GfyStatusResponse
    {
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; set; }

        [JsonProperty("task")]
        public string Task { get; set; }
    }
}