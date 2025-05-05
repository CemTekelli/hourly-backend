using Microsoft.AspNetCore.Http;

namespace Hourly.Core.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder, string fileName);
        void DeleteFile(string filePath);
    }
}