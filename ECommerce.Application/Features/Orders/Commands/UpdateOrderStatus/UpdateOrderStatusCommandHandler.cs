using ECommerce.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order == null)
        {
            return false; // Không tìm thấy đơn hàng
        }

        // (Nâng cao) Thêm logic kiểm tra việc chuyển đổi trạng thái có hợp lệ không
        // Ví dụ: Không thể chuyển từ "Delivered" về "Pending"
        var validStatuses = new[] { "PendingPayment", "Processing", "Shipped", "Delivered", "Canceled" };
        if (!validStatuses.Contains(request.NewStatus, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid order status provided.");
        }

        order.Status = request.NewStatus;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}