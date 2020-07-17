using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;

namespace RedditVideoRotationBot
{
    public class RedditReplyBuilder : IReplyBuilder
    {
        private readonly string ReplyTemplate = $"Video was rotated {{0}}: {{1}}  \r\n\r\n***\r\n^^[usage]({UsageUrl})&nbsp;-&nbsp;[source]({SourceUrl})&nbsp;-&nbsp;[pm&nbsp;me]({PmUrl})";

        private const string UsageUrl = "https://github.com/nmur/reddit-video-rotation-bot/wiki/Detailed-usage-instructions";

        private const string SourceUrl = "https://github.com/nmur/reddit-video-rotation-bot";

        private const string PmUrl = "https://www.reddit.com/message/compose/?to=nmur";

        private readonly IRotationDescriptionDeterminer _rotationDescriptionDeterminer;

        public RedditReplyBuilder(IRotationDescriptionDeterminer rotationDescriptionDeterminer)
        {
            _rotationDescriptionDeterminer = rotationDescriptionDeterminer;
        }

        public string BuildReply(ReplyBuilderParameters replyBuilderParameters)
        {
            ValidateParameters(replyBuilderParameters);

            return string.Format(
                ReplyTemplate,
                _rotationDescriptionDeterminer.GenerateRotationDescriptionMessageArgArgument(replyBuilderParameters.RotationMessageArg),
                replyBuilderParameters.UploadedVideoUrl);
        }

        private static void ValidateParameters(ReplyBuilderParameters replyBuilderParameters)
        {
            if (string.IsNullOrEmpty(replyBuilderParameters.UploadedVideoUrl))
                throw new RedditReplyBuilderException("Uploaded video URL was either null or empty");

            if (string.IsNullOrEmpty(replyBuilderParameters.RotationMessageArg))
                throw new RedditReplyBuilderException("Rotation description was either null or empty");
        }
    }
}
