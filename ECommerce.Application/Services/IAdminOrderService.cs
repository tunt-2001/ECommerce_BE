using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services;

public interface IAdminOrderService
{
    Task<List<AdminOrderListDto>> GetAllAsync();
    Task<AdminOrderDetailDto?> GetByIdAsync(int orderId);
    Task<bool> UpdateStatusAsync(int orderId, string newStatus);
    Task<bool> CancelOrderAsync(int orderId);
}