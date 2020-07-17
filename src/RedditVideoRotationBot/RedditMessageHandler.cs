using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;
using System.Threading.Tasks;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private readonly IMediaProcessor _mediaProcessor;

        private readonly IReplyBuilder _replyBuilder;

        private const string UsernameMentionSubjectString = "username mention";


        public RedditMessageHandler(IRedditClientWrapper redditClientWrapper, IMediaProcessor mediaProcessor, IReplyBuilder replyBuilder)
        {
            _redditClientWrapper = redditClientWrapper;
            _mediaProcessor = mediaProcessor;
            _replyBuilder = replyBuilder;
        }

        public async Task OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            if (NoMessagesInEvent(e)) return;

            foreach (var message in e.NewMessages)
            {
                if (MessageIsUsernameMention(message))
                {
                    try
                    {
                        await RotateAndUploadVideo(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to process video rotation sequence: {ex}");
                    }
                }

                MarkMessageAsRead(message);
            }
        }

        private static bool NoMessagesInEvent(MessagesUpdateEventArgs e)
        {
            return e == null || e.NewMessages == null;
        }

        private static bool MessageIsUsernameMention(Message message)
        {
            return message.Subject == UsernameMentionSubjectString && message.WasComment;
        }

        private async Task RotateAndUploadVideo(Message message)
        {
            var rotationArgument = GetRotationArgument(message);

            var post = GetCommentRootPost(message);
            ThrowExceptionIfPostIsNsfw(post);

            string videoUrl = GetVideoUrlFromPost(post);
            string audioUrl = GetAudioUrlFromPost(post);

            var uploadedVideoUrl = await _mediaProcessor.DownloadAndRotateAndUploadVideo(
                new MediaProcessorParameters
                {
                    RotationArgument = rotationArgument,
                    VideoUrl = videoUrl,
                    AudioUrl = audioUrl
                });

            ReplyToComment(message, _replyBuilder.BuildReply(new ReplyBuilderParameters
            {
                UploadedVideoUrl = uploadedVideoUrl,
                RotationDescription = rotationArgument
            }));
        }

        private static string GetRotationArgument(Message message)
        {
            var messageBodySplitIntoWords = message.Body.Split(' ');
            if (messageBodySplitIntoWords.Length < 2)
                throw new ArgumentNullException("Rotation argument missing from user mention.");

            return messageBodySplitIntoWords[1];
        }

        private static void ThrowExceptionIfPostIsNsfw(Post post)
        {
            if (post.Over18) 
                throw new NotImplementedException("NSFW posts will not be handled until NSFW media upload resource is implemented");
        }

        private string GetVideoUrlFromPost(Post post)
        {
            return RedditPostParser.TryGetVideoUrlFromPost(post);
        }

        private string GetAudioUrlFromPost(Post post)
        {
            return RedditPostParser.TryGetAudioUrlFromPost(post);
        }

        private Post GetCommentRootPost(Message message)
        {
            return _redditClientWrapper.GetCommentRootPost(GetMessageFullname(message)).Listing;
        }

        private void MarkMessageAsRead(Message message)
        {
            _redditClientWrapper.ReadMessage(GetMessageFullname(message));
            Console.WriteLine($"Message was marked as read");
        }

        private void ReplyToComment(Message message, string replyText)
        {
            _redditClientWrapper.ReplyToComment(GetMessageFullname(message), replyText);
            Console.WriteLine("Comment was replied to");
        }

        private static string GetMessageFullname(Message message)
        {
            // User mentions/comments seem to require the t1_x name instead for the read_message API
            return (message.WasComment ? "t1_" : "t4_") + message.Id;
        }
    }
}