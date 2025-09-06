using System.Collections.Generic;
using System.Threading.Tasks;

namespace Futtage.Core.Services
{
    public interface IFileService
    {
        Task<List<string>> SelectVideoFilesAsync();
        Task<string> SelectOutputPathAsync(string defaultName);
        Task<string> SelectImageFileAsync();
        string GetTempFilePath(string extension = ".tmp");
        void DeleteTempFiles();
        bool FileExists(string path);
        long GetFileSize(string path);
    }
}