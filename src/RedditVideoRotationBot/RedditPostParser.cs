using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reddit.Things;
using RedditVideoRotationBot.Exceptions;
using System;

namespace RedditVideoRotationBot
{
    public static class RedditPostParser
    {
        private const string VideoResourcePrefix = "DASH_";

        private const string AudioResourceString = "audio";

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
                throw new RedditPostParserException("Failed to find media url in post");
            }
        }
        public static string TryGetAudioUrlFromPost(Post post)
        {
            return TryGetVideoUrlFromPost(post).Split(VideoResourcePrefix)[0] + AudioResourceString;
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
