using ECommerce.Application.Interfaces;
using ECommerce.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ECommerce.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNewOrderNotificationAsync(string message)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNewOrderNotification", message);
    }
}