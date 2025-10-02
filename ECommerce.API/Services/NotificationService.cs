using ECommerce.API.Hubs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ECommerce.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNewOrderNotificationAsync(string message, Notification newNotification)
    {
        try
        {
            _logger.LogInformation("Attempting to send SignalR notification to 'Admins' group.");

            // Gửi đi cả message (cho toast) và newNotification (cho state)
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNewOrderNotification", message, newNotification);

            _logger.LogInformation("SignalR notification sent successfully.");
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error sending SignalR notification.");
        }
    }
}