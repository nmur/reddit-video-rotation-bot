using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reddit.Things;
using RedditVideoRotationBot.Exceptions;
using System;

namespace RedditVideoRotationBot.Reddit
{
    public static class RedditPostParser
    {
        private const string VideoResourcePrefix = "DASH_";

        private const string AudioResourceString = "audio";

        private const string AudioResourceVariationString = "DASH_audio.mp4";

        public static string TryGetVideoUrlFromPost(Post post)
        {
            try
            {
                var mediaJObject = (JObject)post.Media;
                var media = mediaJObject.ToObject<RedditMedia>();
                return media.RedditVideo.FallbackUrl;
            }
            catch (Exception)
            {
                throw new RedditPostParserException("Failed to find media url in post");
            }
        }
        public static string TryGetAudioUrlFromPost(Post post)
        {
            var videoUrlSplitOnResourcePrefix = TryGetVideoUrlFromPost(post).Split(VideoResourcePrefix);
            return videoUrlSplitOnResourcePrefix[0] + GetAudioResourceString(videoUrlSplitOnResourcePrefix);
        }

        private static string GetAudioResourceString(string[] videoUrlSplitOnResourcePrefix)
        {
            //https://github.com/nmur/reddit-video-rotation-bot/issues/27#issuecomment-653887291
            return videoUrlSplitOnResourcePrefix[1].Contains(".mp4") ? AudioResourceVariationString : AudioResourceString;
        }
    }

    internal class RedditMedia
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
