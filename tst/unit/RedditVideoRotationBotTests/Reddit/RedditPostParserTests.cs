﻿using Xunit;
using Reddit.Things;
using Newtonsoft.Json.Linq;
using RedditVideoRotationBot.Exceptions;
using static FluentAssertions.FluentActions;
using FluentAssertions;
using RedditVideoRotationBot.Reddit;

namespace RedditVideoRotationBotTests
{
    public class RedditPostParserTests
    {
        private const string VideoUrlString = "https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback";

        private const string VideoUrlVariationString = "https://v.redd.it/abcabcabcabc/DASH_1080.mp4?source=fallback";

        private const string AudioUrlString = "https://v.redd.it/abcabcabcabc/audio";

        private const string AudioUrlVariationString = "https://v.redd.it/abcabcabcabc/DASH_audio.mp4";

        private const string MediaString = "{\"reddit_video\":{\"fallback_url\":\"https://v.redd.it/abcabcabcabc/DASH_1080?source=fallback\",\"height\":1080,\"width\":608,\"scrubber_media_url\":\"https://v.redd.it/abcabcabcabc/DASH_96\",\"dash_url\":\"https://v.redd.it/abcabcabcabc/DASHPlaylist.mpd\",\"duration\":8,\"hls_url\":\"https://v.redd.it/abcabcabcabc/HLSPlaylist.m3u8\",\"is_gif\":false,\"transcoding_status\":\"completed\"}}";

        private const string MediaVariationString = "{\"reddit_video\":{\"fallback_url\":\"https://v.redd.it/abcabcabcabc/DASH_1080.mp4?source=fallback\",\"height\":1080,\"width\":608,\"scrubber_media_url\":\"https://v.redd.it/abcabcabcabc/DASH_96.mp4\",\"dash_url\":\"https://v.redd.it/abcabcabcabc/DASHPlaylist.mpd\",\"duration\":8,\"hls_url\":\"https://v.redd.it/abcabcabcabc/HLSPlaylist.m3u8\",\"is_gif\":false,\"transcoding_status\":\"completed\"}}";

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
        public void GivenRedditPostWithVideoWithAlternativeVideoUrl_WhenVideoUrlIsParsedFromPost_ThenUrlIsReturnedSuccessfully()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse(MediaVariationString)
            };

            // Act
            var url = RedditPostParser.TryGetVideoUrlFromPost(post);

            // Assert
            Assert.Equal(VideoUrlVariationString, url);
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

        [Fact]
        public void GivenRedditPostWithAudioWithAlternativeAudioUrl_WhenAudioUrlIsParsedFromPost_ThenUrlIsReturnedSuccessfully()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse(MediaVariationString)
            };

            // Act
            var url = RedditPostParser.TryGetAudioUrlFromPost(post);

            // Assert
            Assert.Equal(AudioUrlVariationString, url);
        }

        [Fact]
        public void GivenRedditPostWithAudio_WhenAudioUrlIsParsedFromPost_ThenUrlIsReturnedSuccessfully()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse(MediaString)
            };

            // Act
            var url = RedditPostParser.TryGetAudioUrlFromPost(post);

            // Assert
            Assert.Equal(AudioUrlString, url);
        }

        [Fact]
        public void GivenRedditPostWithNoAudio_WhenAudioUrlIsParsedFromPost_ThenRedditPostParserExceptionIsThrown()
        {
            // Arrange
            var post = new Post();

            // Act + Assert
            Invoking(() => RedditPostParser.TryGetAudioUrlFromPost(post))
                .Should().Throw<RedditPostParserException>();
        }

        [Fact]
        public void GivenRedditPostWithNonVideoMedia_WhenAudioUrlIsParsedFromPost_ThenRedditPostParserExceptionIsThrown()
        {
            // Arrange
            var post = new Post
            {
                Media = JObject.Parse("{\"some_other_media\":{\"data\":\"value\"}}")
            };

            // Act + Assert
            Invoking(() => RedditPostParser.TryGetAudioUrlFromPost(post))
                .Should().Throw<RedditPostParserException>();
        }
    }
}
