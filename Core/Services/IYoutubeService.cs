using System.Threading.Tasks;
using Futtage.Core.Models;

namespace Futtage.Core.Services
{
    public interface IYouTubeService
    {
        Task<bool> AuthenticateAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserInfo> GetUserInfoAsync();
        Task<string> UploadVideoAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null);
        Task<bool> SetThumbnailAsync(string videoId, string thumbnailPath);
        void ClearCredentials();
    }
}