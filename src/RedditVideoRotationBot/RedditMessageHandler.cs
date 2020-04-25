﻿using Reddit.Controllers.EventArgs;
using Reddit.Things;
using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;
using System.Net;

namespace RedditVideoRotationBot
{
    public class RedditMessageHandler : IRedditMessageHandler
    {
        private readonly IRedditClientWrapper _redditClientWrapper;

        private readonly IVideoDownloader _videoDownloader;

        private const string UsernameMentionSubjectString = "username mention";

        public RedditMessageHandler(IRedditClientWrapper redditClientWrapper, IVideoDownloader videoDownloader)
        {
            _redditClientWrapper = redditClientWrapper;
            _videoDownloader = videoDownloader;
        }

        public void OnUnreadMessagesUpdated(object sender, MessagesUpdateEventArgs e)
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
                    DeleteVideoFileIfPresent();

                    _videoDownloader.DownloadFromUrl(videoUrl);

                    // rotate video test
                    if (File.Exists("video.mp4"))
                    {
                        Console.WriteLine($"video.mp4 found");
                        System.Diagnostics.Process proc = new System.Diagnostics.Process
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "ffmpeg",
                                Arguments = "-i video.mp4 -c copy -metadata:s:v:0 rotate=90 video_rotated.mp4",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            }
                        };
                    
                        Console.WriteLine($"Starting video rotation...");
                        proc.Start();
                        Console.WriteLine($"Started video rotation, waiting for process to complete...");
                        proc.WaitForExit();
                        Console.WriteLine($"Process complete with exit code: {proc.ExitCode}");
                    }

                    if (File.Exists("video_rotated.mp4"))
                    {
                        Console.WriteLine($"video rotated successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"ERROR: video rotated unsuccessfully!");
                    }

                    ReplyToComment(message);
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

        private static void DeleteVideoFileIfPresent()
        {
            if (File.Exists("video.mp4"))
            {
                File.Delete("video.mp4");
            }
        }

        private void ReplyToComment(Message message)
        {
            _redditClientWrapper.ReplyToComment(GetMessageFullname(message));
            Console.WriteLine($"Comment was replied to");
        }

        private static string GetMessageFullname(Message message)
        {
            // User mentions/comments seem to require the t1_x name instead for the read_message API
            return (message.WasComment ? "t1_" : "t4_") + message.Id;
        }
    }
}