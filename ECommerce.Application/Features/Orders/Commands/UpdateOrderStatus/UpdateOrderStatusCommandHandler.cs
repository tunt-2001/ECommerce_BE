using ECommerce.Application.Interfaces;
using MediatR;

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
            return false;
        }

        // Có thể thêm logic kiểm tra xem việc chuyển trạng thái có hợp lệ không
        // Ví dụ: không thể chuyển từ "Shipped" về "Pending"
        order.Status = request.NewStatus;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}