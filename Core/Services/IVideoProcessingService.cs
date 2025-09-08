using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Futtage.Core.Models;

namespace Futtage.Core.Services
{
    public interface IVideoProcessingService
    {
        Task<string> ConcatenateAsync(List<string> videoPaths, string outputPath, IProgress<ProcessingProgress>? progress = null);
        Task<string> TrimAsync(string inputPath, TimeSpan start, TimeSpan end, string outputPath, IProgress<ProcessingProgress>? progress = null);
        Task<VideoInfo> GetVideoInfoAsync(string videoPath);
        Task<List<VideoInfo>> GetMultipleVideoInfoAsync(List<string> videoPaths);
        bool ValidateVideoFile(string videoPath);
    }
}