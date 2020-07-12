using RedditVideoRotationBot.Exceptions;
using RedditVideoRotationBot.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RedditVideoRotationBot
{
    public class MediaProcessor : IMediaProcessor
    {
        private readonly IVideoDownloader _videoDownloader;

        private readonly IAudioDownloader _audioDownloader;

        private readonly IMediaMuxer _mediaMuxer;

        private readonly IVideoRotator _videoRotator;

        private readonly IVideoUploader _videoUploader;

        private const string VideoFileNameString = "video.mp4";

        private const string AudioFileNameString = "audio.mp4";

        private const string RotatedVideoFileNameString = "video_rotated.mp4";

        public MediaProcessor(IVideoDownloader videoDownloader, IAudioDownloader audioDownloader, IMediaMuxer mediaMuxer, IVideoRotator videoRotator, IVideoUploader videoUploader)
        {
            _videoDownloader = videoDownloader;
            _audioDownloader = audioDownloader;
            _mediaMuxer = mediaMuxer;
            _videoRotator = videoRotator;
            _videoUploader = videoUploader;
        }

        public async Task<string> DownloadAndRotateAndUploadVideo(MediaProcessorParameters mediaProcessorParameters)
        {
            try
            {
                DeleteMediaFilesIfPresent();
                DownloadVideoFromUrl(mediaProcessorParameters.VideoUrl);
                DownloadAudioFromUrl(mediaProcessorParameters.AudioUrl);
                CombineVideoAndAudio();
                RotateVideo(mediaProcessorParameters.RotationArgument);
                return await UploadVideoAsync();
            }
            catch (Exception ex)
            {
                throw new MediaProcessorException("Error during download or rotate or upload", ex);
            }
        }

        private static void DeleteMediaFilesIfPresent()
        {
            if (File.Exists(VideoFileNameString)) File.Delete(VideoFileNameString);
            if (File.Exists(AudioFileNameString)) File.Delete(AudioFileNameString);
            if (File.Exists(RotatedVideoFileNameString)) File.Delete(RotatedVideoFileNameString);
        }

        private void CombineVideoAndAudio()
        {
            _mediaMuxer.CombineVideoAndAudio();
        }

        private void DownloadAudioFromUrl(string url)
        {
            _audioDownloader.DownloadFromUrl(url);
        }

        private void DownloadVideoFromUrl(string url)
        {
            _videoDownloader.DownloadFromUrl(url);
        }

        private void RotateVideo(string rotationArg)
        {
            _videoRotator.Rotate(rotationArg);
        }

        private async Task<string> UploadVideoAsync()
        {
            return await _videoUploader.UploadAsync();
        }
    }
}
