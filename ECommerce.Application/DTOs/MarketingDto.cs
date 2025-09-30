namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO dùng để nhận dữ liệu từ request body cho việc gửi bản tin (newsletter).
/// </summary>
public class NewsletterDto
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// URL của hình ảnh quảng cáo (tùy chọn).
    /// </summary>
    public string? ImageUrl { get; set; }
}