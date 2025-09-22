using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService;

    public OrderService(IApplicationDbContext context, IQRCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    public async Task<OrderResultDto> CreateOrderAsync(string userId, OrderRequestDto request)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            PaymentMethod = request.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            Status = "PendingPayment"
        };

        decimal totalAmount = 0;
        var productIds = request.OrderItems.Select(i => i.ProductId).ToList();
        var productsInCart = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        foreach (var item in request.OrderItems)
        {
            var product = productsInCart.FirstOrDefault(p => p.Id == item.ProductId);

            if (product == null || product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Product '{product?.Name ?? "ID: " + item.ProductId}' is out of stock or does not exist.");
            }

            var orderDetail = new OrderDetail
            {
                Order = order,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            };
            order.OrderDetails.Add(orderDetail);

            product.Stock -= item.Quantity;
            totalAmount += product.Price * item.Quantity;
        }

        order.TotalAmount = totalAmount;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(default);

        string? qrCode = null;
        if (request.PaymentMethod.Equals("QRCode", StringComparison.OrdinalIgnoreCase))
        {
            qrCode = _qrCodeService.GenerateQRCodeBase64(totalAmount, $"Pay for Order #{order.Id}");
        }

        return new OrderResultDto { OrderId = order.Id, QRCodeImageBase64 = qrCode };
    }

    public async Task<List<UserOrderDto>> GetMyOrdersAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new UserOrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.OrderDetails.Count
            })
            .ToListAsync();
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int orderId, string userId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId && o.UserId == userId)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Select(o => new OrderDetailDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
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
}