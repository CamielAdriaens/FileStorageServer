using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using FileStorage.Services;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Get all files
        [HttpGet()]
        public async Task<IActionResult> GetFiles()
        {
            var fileInfos = await _fileService.GetFilesAsync();
            return Ok(fileInfos);
        }

        // Upload a file
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not provided");

            using var stream = file.OpenReadStream();
            var fileId = await _fileService.UploadFileAsync(stream, file.FileName);

            return Ok(new { FileId = fileId.ToString() });
        }

        // Download a file
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest("Invalid file ID");

            var fileStream = await _fileService.DownloadFileAsync(objectId);
            if (fileStream == null)
                return NotFound();

            return File(fileStream, "application/octet-stream", $"{id}.file");
        }

        // Delete a file
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest("Invalid file ID");

            await _fileService.DeleteFileAsync(objectId);
            return NoContent();
        }
    }
}
