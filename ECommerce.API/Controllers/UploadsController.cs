using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<UploadsController> _logger;
    private readonly IConfiguration _configuration;

    public UploadsController(
        IWebHostEnvironment webHostEnvironment,
        ILogger<UploadsController> logger,
        IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string type = "products")
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Upload attempt failed: No file was uploaded.");
            return BadRequest("No file uploaded.");
        }

        if (file.Length > 5 * 1024 * 1024) // 5 MB
        {
            _logger.LogWarning("Upload attempt failed: File size exceeds the 5MB limit. File size: {FileSize}", file.Length);
            return BadRequest("File size exceeds the 5MB limit.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Upload attempt failed: Invalid file type uploaded: {FileExtension}", extension);
            return BadRequest("Invalid file type. Only .jpg, .jpeg, .png, .gif are allowed.");
        }

        try
        {
            var subfolder = type.Equals("marketing", StringComparison.OrdinalIgnoreCase) ? "marketing" : "products";
            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", subfolder);

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // === SỬA LẠI LOGIC TẠO URL Ở ĐÂY ===
            // 1. Ưu tiên lấy URL công khai từ cấu hình (dùng cho ngrok)
            //var baseUrl = _configuration["PublicUrl"];
            var baseUrl = "https://localhost:7066";

            // 2. Nếu không có trong cấu hình, tự động lấy từ request hiện tại (dùng cho localhost hoặc server thật)
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = $"{Request.Scheme}://{Request.Host}";
            }

            // 3. Tạo URL đầy đủ
            var imageUrl = $"{baseUrl}/images/{subfolder}/{uniqueFileName}";

            _logger.LogInformation("File uploaded successfully. Public URL: {ImageUrl}", imageUrl);
            return Ok(new { imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during file upload.");
            return StatusCode(500, "An internal server error occurred during file upload.");
        }
    }
}