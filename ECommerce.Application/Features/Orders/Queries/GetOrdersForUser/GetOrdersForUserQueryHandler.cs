using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersForUser;

public class GetOrdersForUserQueryHandler : IRequestHandler<GetOrdersForUserQuery, List<UserOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersForUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserOrderDto>> Handle(GetOrdersForUserQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == request.UserId)
            .Include(o => o.OrderDetails) // Nạp các chi tiết đơn hàng để đếm
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new UserOrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.OrderDetails.Count // Đếm số loại sản phẩm trong đơn
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}