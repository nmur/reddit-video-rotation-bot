using Xunit;
using Reddit.Things;
using Newtonsoft.Json.Linq;
using RedditVideoRotationBot;
using RedditVideoRotationBot.Exceptions;
using System;
using static FluentAssertions.FluentActions;
using FluentAssertions;

namespace RedditVideoRotationBotTests
{
    public class RedditPostParserTests
    {
        private const string VideoUrlString = "https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback";

        private const string MediaString = "{\"reddit_video\":{\"fallback_url\":\"https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback\",\"height\":1080,\"width\":608,\"scrubber_media_url\":\"https://v.redd.it/abcabcabcabc/DASH_96\",\"dash_url\":\"https://v.redd.it/abcabcabcabc/DASHPlaylist.mpd\",\"duration\":8,\"hls_url\":\"https://v.redd.it/abcabcabcabc/HLSPlaylist.m3u8\",\"is_gif\":false,\"transcoding_status\":\"completed\"}}";

        [Fact]
        public void GivenRedditPostWithVideo_WhenVideoUrlIsParsedFromPost_ThenUrlIsReturnedSuccessfully()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse(MediaString)
            };

            // Act
            var url = RedditPostParser.TryGetVideoUrlFromPost(post);

            // Assert
            Assert.Equal(VideoUrlString, url);
        }

        [Fact]
        public void GivenRedditPostWithNoVideo_WhenVideoUrlIsParsedFromPost_ThenRedditPostParserExceptionIsThrown()
        {
            // Arrange
            var post = new Post();

            // Act + Assert
            Invoking(() => RedditPostParser.TryGetVideoUrlFromPost(post))
                .Should().Throw<RedditPostParserException>();
        }

        [Fact]
        public void GivenRedditPostWithNonVideoMedia_WhenVideoUrlIsParsedFromPost_ThenRedditPostParserExceptionIsThrown()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse("{\"some_other_media\":{\"data\":\"value\"}}")
            };

            // Act + Assert
            Invoking(() => RedditPostParser.TryGetVideoUrlFromPost(post))
                .Should().Throw<RedditPostParserException>();
        }
    }
}
