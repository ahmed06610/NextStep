using Microsoft.AspNetCore.Http;
using NextStep.Core.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace NextStep.Core.Services
{
    public class FileService : IFileService
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _webHostEnvironment;
        private const string BaseFilesFolder = "ApplicationFiles";

        public FileService(IHostingEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveApplicationFileAsync(IFormFile file, int applicationId)
        {
            if (file == null || file.Length == 0)
                return null;

            // Create application-specific folder
            var applicationFolder = Path.Combine(BaseFilesFolder, $"App_{applicationId}");
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, applicationFolder);
            Directory.CreateDirectory(uploadsFolder);

            // Generate safe filename
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path
            return Path.Combine(applicationFolder, uniqueFileName).Replace("\\", "/");
        }

        public void DeleteApplicationFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var fullPath = GetFileFullPath(filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public string GetFileFullPath(string relativePath)
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
        }
    }
}
