﻿using Reddit.Controllers.EventArgs;
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

        private readonly IAudioDownloader _audioDownloader;

        private readonly IMediaMuxer _mediaMuxer;

        private readonly IVideoRotator _videoRotator;

        private readonly IVideoUploader _videoUploader;

        private const string UsernameMentionSubjectString = "username mention";

        private const string VideoFileNameString = "video.mp4";

        private const string AudioFileNameString = "audio.mp4";

        private const string RotatedVideoFileNameString = "video_rotated.mp4";

        public RedditMessageHandler(IRedditClientWrapper redditClientWrapper, IVideoDownloader videoDownloader, IAudioDownloader audioDownloader, IMediaMuxer mediaMuxer, IVideoRotator videoRotator, IVideoUploader videoUploader)
        {
            _redditClientWrapper = redditClientWrapper;
            _videoDownloader = videoDownloader;
            _audioDownloader = audioDownloader;
            _mediaMuxer = mediaMuxer;
            _videoRotator = videoRotator;
            _videoUploader = videoUploader;
        }

        public async Task OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
        {
            if (e == null || e.NewMessages == null) return;

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

        private static bool MessageIsUsernameMention(Message message)
        {
            return message.Subject == UsernameMentionSubjectString && message.WasComment;
        }

        private async Task RotateAndUploadVideo(Message message)
        {
            var rotationArgument = GetRotationArgument(message);

            var post = GetCommentRootPost(message);
            ThrowExceptionIfPostIsNsfw(post);

            //delete video file if there's one already. only process one file at a time for now
            DeleteMediaFilesIfPresent();

            string videoUrl = GetVideoUrlFromPost(post);
            string audioUrl = GetAudioUrlFromPost(post);

            _videoDownloader.DownloadFromUrl(videoUrl);
            _audioDownloader.DownloadFromUrl(audioUrl);
            _mediaMuxer.CombineVideoAndAudio();
            _videoRotator.Rotate(rotationArgument);
            var uploadedVideoUrl = await _videoUploader.UploadAsync();

            ReplyToCommentWithUploadedVideoUrl(message, uploadedVideoUrl);
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

        private static void DeleteMediaFilesIfPresent()
        {
            if (File.Exists(VideoFileNameString)) File.Delete(VideoFileNameString);
            if (File.Exists(AudioFileNameString)) File.Delete(AudioFileNameString);
            if (File.Exists(RotatedVideoFileNameString)) File.Delete(RotatedVideoFileNameString);
        }

        private void ReplyToCommentWithUploadedVideoUrl(Message message, string url)
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