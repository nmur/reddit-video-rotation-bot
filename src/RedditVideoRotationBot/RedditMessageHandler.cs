using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private readonly IVideoDownloader _videoDownloader;

        private readonly IVideoRotator _videoRotator;

        private readonly IVideoUploader _videoUploader;

        private const string UsernameMentionSubjectString = "username mention";

        public RedditMessageHandler(IRedditClientWrapper redditClientWrapper, IVideoDownloader videoDownloader, IVideoRotator videoRotator, IVideoUploader videoUploader)
        {
            _redditClientWrapper = redditClientWrapper;
            _videoDownloader = videoDownloader;
            _videoRotator = videoRotator;
            _videoUploader = videoUploader;
        }

        public async Task OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            if (e == null || e.NewMessages == null) return;

            foreach (var message in e.NewMessages)
            {
                Console.WriteLine($"Message received from {message.Author}");

                if (MessageIsUsernameMention(message)) //TODO: refactor this body when scope and form becomes more apparent
                {
                    Console.WriteLine($"Message was a user mention");
                    string videoUrl = RedditPostParser.TryGetVideoUrlFromPost(GetCommentRootPost(message));
                    Console.WriteLine($"videoUrl: {videoUrl}");

                    //delete video file if there's one already. only process one file at a time for now
                    DeleteVideoFilesIfPresent();

                    _videoDownloader.DownloadFromUrl(videoUrl);
                    _videoRotator.Rotate();
                    var uploadedVideoUrl = await _videoUploader.UploadAsync();

                    ReplyToComment(message, uploadedVideoUrl);
                }

                MarkMessageAsRead(message);
            }
        }

        private static bool MessageIsUsernameMention(Message message)
        {
            return message.Subject == UsernameMentionSubjectString && message.WasComment;
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

        private static void DeleteVideoFilesIfPresent()
        {
            if (File.Exists("video.mp4"))
            {
                File.Delete("video.mp4");
            }
            if (File.Exists("video_rotated.mp4"))
            {
                File.Delete("video_rotated.mp4");
            }
        }

        private void ReplyToComment(Message message, string url)
        {
            _redditClientWrapper.ReplyToComment(GetMessageFullname(message), url);
            Console.WriteLine($"Comment was replied to");
        }

        private static string GetMessageFullname(Message message)
        {
            // User mentions/comments seem to require the t1_x name instead for the read_message API
            return (message.WasComment ? "t1_" : "t4_") + message.Id;
        }
    }
}