using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class NotificationPersistenceService : INotificationPersistenceService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<NotificationPersistenceService> _logger;

    public NotificationPersistenceService(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<NotificationPersistenceService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<List<Notification>> CreateNotificationsForAdminsAsync(string message, string entityType, int entityId)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        if (!admins.Any())
        {
            _logger.LogWarning("CreateNotificationsForAdminsAsync: No users with 'Admin' role found.");
            return new List<Notification>(); // Trả về danh sách rỗng
        }

        _logger.LogInformation("Creating notifications for {AdminCount} admins.", admins.Count);

        var notifications = admins.Select(admin => new Notification
        {
            UserId = admin.Id,
            Message = message,
            IsRead = false,
            CreatedDate = DateTime.UtcNow,
            EntityType = entityType,
            RelatedEntityId = entityId.ToString()
        }).ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync(default);

        // Trả về danh sách các notification vừa được tạo (đã có ID từ DB)
        return notifications;
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task MarkNotificationsAsReadAsync(string userId, List<int> notificationIds)
    {
        if (notificationIds == null || !notificationIds.Any())
        {
            return;
        }

        var notificationsToUpdate = await _context.Notifications
            .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
            .ToListAsync();

        if (notificationsToUpdate.Any())
        {
            foreach (var notification in notificationsToUpdate)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync(default);
            _logger.LogInformation("Marked {Count} notifications as read for User {UserId}.", notificationsToUpdate.Count, userId);
        }
    }

    public async Task<List<Notification>> GetAllAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedDate)
            .Take(20) // Giới hạn lấy 20 thông báo gần nhất để tối ưu
            .ToListAsync();
    }
}