using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface INotificationService
{
    Task SendNewOrderNotificationAsync(string message);
}