using ECommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Application.Services;

/// <summary>
/// Interface cho dịch vụ quản lý việc lưu trữ và truy xuất thông báo từ database.
/// </summary>
public interface INotificationPersistenceService
{
    /// <summary>
    /// Tạo các bản ghi thông báo trong database cho tất cả người dùng có quyền "Admin".
    /// </summary>
    /// <param name="message">Nội dung của thông báo.</param>
    /// <returns>Một danh sách các đối tượng Notification vừa được tạo và lưu.</returns>
    Task<List<Notification>> CreateNotificationsForAdminsAsync(string message, string entityType, int entityId);

    /// <summary>
    /// Lấy tất cả các thông báo chưa đọc của một người dùng cụ thể.
    /// </summary>
    /// <param name="userId">ID của người dùng.</param>
    /// <returns>Danh sách các thông báo chưa đọc.</returns>
    Task<List<Notification>> GetUnreadNotificationsAsync(string userId);

    /// <summary>
    /// Đánh dấu một danh sách các thông báo là đã đọc cho một người dùng cụ thể.
    /// </summary>
    /// <param name="userId">ID của người dùng.</param>
    /// <param name="notificationIds">Danh sách ID của các thông báo cần đánh dấu.</param>
    Task MarkNotificationsAsReadAsync(string userId, List<int> notificationIds);
    Task<List<Notification>> GetAllAsync(string userId);
}