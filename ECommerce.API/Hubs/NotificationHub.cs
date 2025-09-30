using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ECommerce.API.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
}