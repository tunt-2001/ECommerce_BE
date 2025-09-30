using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using System.Threading.Tasks;

namespace ECommerce.Application.Services;

/// <summary>
/// Interface cho các dịch vụ marketing dành cho Admin.
/// </summary>
public interface IAdminMarketingService
{
    /// <summary>
    /// Gửi một bản tin (newsletter) đến tất cả người dùng trong hệ thống.
    /// </summary>
    /// <param name="dto">Đối tượng chứa thông tin của bản tin.</param>
    Task SendNewsletterAsync(NewsletterDto dto);
    Task<List<Newsletter>> GetNewsletterHistoryAsync();
}