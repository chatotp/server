using Microsoft.AspNetCore.Mvc;
using server.Services;
using System.Xml.Linq;

namespace server.Controllers
{
    [ApiController]
    [Route("api")]
    public class FileUploadController : ControllerBase
    {
        private readonly FileUploadService _fileUploadService;
        private readonly ChatAuthenticationService _chatAuthenticationService;

        public FileUploadController(FileUploadService fileUploadService, ChatAuthenticationService authenticationService)
        {
            _fileUploadService = fileUploadService;
            _chatAuthenticationService = authenticationService;
        }

        [HttpGet("download/{hash}")]
        public IActionResult Download(string hash, string name)
        {
            var filePath = _fileUploadService.GetFilePath(hash);

            if (filePath == null)
            {
                return NotFound("File not found");
            }

            var contentType = _fileUploadService.GetFileContentType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            string fileName = string.IsNullOrEmpty(name) ? Path.GetFileName(filePath) : name;
            
            Console.WriteLine($"[GET]: {name} - {hash}");
            return File(fileStream, contentType, fileName);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string hash)
        {
            var result = await _fileUploadService.UploadFileAsync(file, hash);

            if (!result.Item1)
            {
                return BadRequest(result.Item2);
            }

            Console.WriteLine($"[POST]: {file.FileName} - {hash}");
            return Ok(file.FileName);
        }

    }
}
