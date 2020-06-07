using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reddit.Things;
using System;

namespace RedditVideoRotationBot
{
    public static class RedditPostParser
    {
        public static string TryGetVideoUrlFromPost(Post post)
        {
            try
            {
                var mediaJObject = (JObject)post.Media;
                var media = mediaJObject.ToObject<Media>();
                return media.RedditVideo.FallbackUrl;
            }
            catch (Exception)
            {
                Console.WriteLine("No video found in post");
                return "";
            }
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
