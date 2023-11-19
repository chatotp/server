using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace server.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsDir;
        private readonly Dictionary<string, string> _allowedMimeTypes = new Dictionary<string, string>
        {
            { ".txt", "text/plain" },
            { ".doc", "application/vnd.openxmlformats-officedocument.wordprocessingml" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml" },
            { ".pdf", "application/pdf" }
        };

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadsDir = Path.Combine(_environment.ContentRootPath, "uploads");
        }

        public string? GetFilePath(string hash)
        {
            var filePath = Path.Combine(_uploadsDir, hash);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return filePath;
        }

        public async Task<(bool, string)> UploadFileAsync(IFormFile file, string providedHash)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File is null or empty");
            }

            /* TODO: Fix CheckFile -- failing even if file type in dict */
            /*if (!CheckFile(file))
            {
                return (false, "File type not allowed");    
            }*/

            string computedHash = await ComputeFileHashAsync(file);

            // Compare the provided hash with the generated hash of the file
            if (!string.Equals(providedHash, computedHash, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Provided hash does not match.");
            }

            var filePath = Path.Combine(_uploadsDir, computedHash);

            // Ensure the directory exists before saving the file
            if (!Directory.Exists(_uploadsDir))
            {
                Directory.CreateDirectory(_uploadsDir);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, computedHash);
        }

        private bool CheckFile(IFormFile file)
        {
            var contentTypeProvider = new FileExtensionContentTypeProvider();

            // Validate by extension
            // ToLowerInvariant for non-language specific data
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (_allowedMimeTypes.ContainsKey(extension))
            {
                return false;
            }

            // Validate by content type in case of forgery
            Stream stream = file.OpenReadStream();
            var contentType = contentTypeProvider.TryGetContentType(file.FileName, out string result) ? result : null;
            var allowedMimeType = _allowedMimeTypes[extension];

            return contentType != null && contentType.Equals(allowedMimeType);
        }

        public string GetFileContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        private async Task<string> ComputeFileHashAsync(IFormFile file)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = file.OpenReadStream())
            {
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
