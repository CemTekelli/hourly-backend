using Hourly.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Hourly.Infrastructure.Storage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public FileStorageService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"];
            _baseUrl = configuration["FileStorage:BaseUrl"];

            // Créer le répertoire de base s'il n'existe pas
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder, string fileName)
        {
            // Créer le dossier si nécessaire
            var directoryPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Chemin complet du fichier
            var filePath = Path.Combine(directoryPath, fileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retourner l'URL complète du fichier
            return $"{_baseUrl}/{folder}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            // Extraire le chemin relatif à partir de l'URL
            var url = new Uri(filePath);
            var relativePath = url.AbsolutePath;

            // Construire le chemin physique
            var physicalPath = Path.Combine(_basePath, relativePath.TrimStart('/'));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}