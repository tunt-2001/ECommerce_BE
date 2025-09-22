using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data; // Cần using này để inject DbContext thật
using Microsoft.AspNetCore.Identity; // Cần cho UserManager
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        ApplicationDbContext context,
        IQRCodeService qrCodeService,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        ILogger<OrderService> logger)
    {
        _context = context;
        _qrCodeService = qrCodeService;
        _emailService = emailService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<OrderResultDto> CreateOrderAsync(string userId, OrderRequestDto request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        _logger.LogInformation("Beginning transaction for creating order for User {UserId}", userId);

        try
        {
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = request.ShippingAddress,
                PaymentMethod = request.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                Status = "Processing"
            };

            decimal totalAmount = 0;
            var productIds = request.OrderItems.Select(i => i.ProductId).ToList();

            // Lấy tất cả sản phẩm trong giỏ hàng trong một lần truy vấn DB để tối ưu
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
                    Price = product.Price // Lấy giá tại thời điểm mua
                };
                order.OrderDetails.Add(orderDetail);

                product.Stock -= item.Quantity; // Giảm tồn kho
                totalAmount += product.Price * item.Quantity;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);

            var savedChanges = await _context.SaveChangesAsync(default);
            _logger.LogInformation("{Count} changes saved to the database within the transaction.", savedChanges);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for Order {OrderId}", order.Id);

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Email != null)
            {
                try
                {
                    var subject = $"Your E-Commerce Store Order #{order.Id} is confirmed!";
                    var body = $"<h1>Thank you for your order!</h1><p>We are now processing your order #{order.Id}. You can view its status in your 'My Orders' page.</p>";
                    await _emailService.SendEmailAsync(user.Email, subject, body);
                    _logger.LogInformation("Confirmation email sent successfully to {Email} for Order {OrderId}", user.Email, order.Id);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send confirmation email for Order {OrderId}", order.Id);
                }
            }

            string? qrCode = null;
            if (request.PaymentMethod.Equals("QRCode", StringComparison.OrdinalIgnoreCase))
            {
                qrCode = _qrCodeService.GenerateQRCodeBase64(totalAmount, $"Pay for Order #{order.Id}");
            }

            return new OrderResultDto { OrderId = order.Id, QRCodeImageBase64 = qrCode };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating order for User {UserId}. Rolling back transaction.", userId);
            await transaction.RollbackAsync();
            throw;
        }
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
                ItemCount = o.OrderDetails.Sum(d => d.Quantity) // Tính tổng số lượng sản phẩm
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
                PaymentMethod = o.PaymentMethod,
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