using Combophoto.Api.BLL.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Combophoto.Api.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FilesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("{key}/url")]
        public async Task<IActionResult> GetPresignedUrl(string key, [FromQuery] double expiresHours = 1)
        {
            try
            {
                var url = await _storageService.GetPresignedUrlAsync(key, expiresHours);
                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Генерируем уникальное имя, чтобы избежать перезаписи
                var uniqueKey = $"{Guid.NewGuid()}_{file.FileName}";
                var resultKey = await _storageService.UploadFileAsync(file, uniqueKey);

                double expiresHours = 1;
                var url = await _storageService.GetPresignedUrlAsync(uniqueKey, expiresHours);

                return Ok(new { Message = "Upload successful", Url = url });                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
