using ECommerce.Application.Features.Orders.Commands.CreateOrder; 
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService; // 1. Inject QRCodeService

    // 2. Cập nhật constructor để nhận IQRCodeService
    public CreateOrderCommandHandler(IApplicationDbContext context, IQRCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    public async Task<CreateOrderResultDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // === BƯỚC 1: TẠO ĐỐI TƯỢNG ORDER BAN ĐẦU ===
        var order = new Order
        {
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress,
            PaymentMethod = request.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            // Đặt trạng thái ban đầu là "Chờ thanh toán"
            Status = "PendingPayment"
        };

        decimal totalAmount = 0;

        // === BƯỚC 2: XỬ LÝ CÁC SẢN PHẨM TRONG ĐƠN HÀNG ===
        foreach (var item in request.OrderItems)
        {
            // Tìm sản phẩm trong DB
            var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);

            // Kiểm tra nghiệp vụ: Sản phẩm có tồn tại và còn đủ hàng không
            if (product == null || product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Product '{product?.Name ?? "ID: " + item.ProductId}' is not available in the requested quantity.");
            }

            // Tạo một dòng chi tiết đơn hàng
            var orderDetail = new OrderDetail
            {
                Order = order, // Gán quan hệ
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price // Lấy giá tại thời điểm mua
            };
            order.OrderDetails.Add(orderDetail);

            // Giảm số lượng tồn kho
            product.Stock -= item.Quantity;

            // Cộng dồn vào tổng tiền
            totalAmount += product.Price * item.Quantity;
        }

        order.TotalAmount = totalAmount;

        // === BƯỚC 3: LƯU ĐƠN HÀNG VÀO DATABASE ===
        // Phải lưu trước để có được OrderId cho mã QR
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // === BƯỚC 4: TẠO MÃ QR (NẾU CẦN) ===
        string? qrCodeBase64 = null;
        if (request.PaymentMethod.Equals("QRCode", StringComparison.OrdinalIgnoreCase))
        {
            // Nội dung chuyển khoản nên là mã đơn hàng để dễ đối soát
            string qrContent = $"Pay Order {order.Id}";
            qrCodeBase64 = _qrCodeService.GenerateQRCodeBase64(order.TotalAmount, qrContent);
        }

        // === BƯỚC 5: TRẢ VỀ KẾT QUẢ CHO API CONTROLLER ===
        return new CreateOrderResultDto
        {
            OrderId = order.Id,
            QRCodeImageBase64 = qrCodeBase64
        };
    }
}