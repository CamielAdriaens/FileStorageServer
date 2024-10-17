using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FileStorage.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Google JWT authorization
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService; // MongoDB file service
        private readonly IUserService _userService; // SQL user service

        public FilesController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        // Get user-specific files
        [HttpGet("secure-files")]
        public async Task<IActionResult> GetUserFiles()
        {
            var googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (googleId == null)
                return Unauthorized("Google ID not found");

            var userFiles = await _userService.GetUserFilesAsync(googleId);

            return Ok(userFiles); // Only metadata is returned, file content is stored in MongoDB
        }

        // Upload a file to MongoDB and store metadata in SQL
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not provided");

            var googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (googleId == null)
                return Unauthorized("Google ID not found");

            // Get or create the user
            var user = await _userService.GetOrCreateUserByGoogleIdAsync(googleId, email, name);

            // Store the file in MongoDB
            using var stream = file.OpenReadStream();
            var mongoFileId = await _fileService.UploadFileAsync(stream, file.FileName);

            // Store metadata in SQL
            await _userService.AddUserFileAsync(googleId, mongoFileId.ToString(), file.FileName);

            return Ok(new { FileId = mongoFileId.ToString() });
        }
    }
}
