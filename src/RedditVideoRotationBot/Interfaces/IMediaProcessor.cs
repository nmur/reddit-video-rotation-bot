using System.Threading.Tasks;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IMediaProcessor
    {
        Task<string> DownloadAndRotateAndUploadVideo(MediaProcessorParameters mediaProcessorParameters);
    }
}
