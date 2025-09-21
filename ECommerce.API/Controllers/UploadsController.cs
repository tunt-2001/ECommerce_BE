using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; // Cần inject IWebHostEnvironment
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Chỉ Admin mới được upload
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UploadsController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Kiểm tra loại file (chỉ cho phép ảnh)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            return BadRequest("Invalid file type. Only images are allowed.");
        }

        // wwwroot là thư mục gốc để phục vụ các file tĩnh
        var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

        // Tạo thư mục nếu nó chưa tồn tại
        if (!Directory.Exists(uploadsFolderPath))
        {
            Directory.CreateDirectory(uploadsFolderPath);
        }

        // Tạo một tên file duy nhất để tránh trùng lặp
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

        // Lưu file vào server
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Trả về URL công khai của file ảnh
        // Request.Scheme: http hoặc https
        // Request.Host: localhost:44399
        // URL sẽ có dạng: https://localhost:44399/images/products/xxxxxxxx-xxxx.jpg
        var imageUrl = $"{Request.Scheme}://{Request.Host}/images/products/{uniqueFileName}";

        return Ok(new { imageUrl });
    }
}