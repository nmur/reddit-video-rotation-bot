using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reddit.Things;

namespace RedditVideoRotationBot
{
    public static class RedditPostParser
    {
        public static string TryGetVideoUrlFromPost(Post post)
        {
            var mediaJObject = (JObject)post.Media;
            var media = mediaJObject.ToObject<Media>();
            return media.RedditVideo.FallbackUrl;
        }
    }

    internal class Media
    {
        [JsonProperty("reddit_video")]
        public RedditVideo RedditVideo;
    }

    internal class RedditVideo
    {
        [JsonProperty("fallback_url")]
        public string FallbackUrl;
    }
}
