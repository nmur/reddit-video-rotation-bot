using System.Threading.Tasks;

namespace RedditVideoRotationBot.Interfaces
{
    public interface IVideoUploader
    {
        Task UploadAsync();
    }
}
