using ECommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public interface IOrderService
    {
        Task<OrderResultDto> CreateOrderAsync(string userId, OrderRequestDto orderRequest);
        Task<List<UserOrderDto>> GetMyOrdersAsync(string userId);
        Task<OrderDetailDto?> GetOrderByIdAsync(int orderId, string userId);
    }
}
