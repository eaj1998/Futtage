using System.Threading.Tasks;
using Futtage.Core.Models;

namespace Futtage.Core.Services
{
    public interface IYouTubeService : IDisposable
    {
        Task<bool> AuthenticateAsync();
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserInfo> GetUserInfoAsync();
        Task<string> UploadVideoAsync(YouTubeUploadRequest request, IProgress<ProcessingProgress>? progress = null);
        Task<bool> SetThumbnailAsync(string videoId, string thumbnailPath);
        void ClearCredentials();
    }
}