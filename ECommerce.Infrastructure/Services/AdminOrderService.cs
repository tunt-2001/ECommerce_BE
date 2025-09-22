using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class AdminOrderService : IAdminOrderService
{
    private readonly IApplicationDbContext _context;

    public AdminOrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminOrderListDto>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.User) // Join để lấy tên người dùng
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new AdminOrderListDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            })
            .ToListAsync();
    }

    public async Task<AdminOrderDetailDto?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Select(o => new AdminOrderDetailDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                CustomerId = o.User.Id,
                CustomerEmail = o.User.Email!,
                Items = o.OrderDetails.Select(od => new OrderItemDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }

        order.Status = newStatus;
        return await _context.SaveChangesAsync(default) > 0;
    }
}