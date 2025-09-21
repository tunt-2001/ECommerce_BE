using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersForAdmin;

public class GetOrdersForAdminQueryHandler : IRequestHandler<GetOrdersForAdminQuery, List<AdminOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersForAdminQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminOrderDto>> Handle(GetOrdersForAdminQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.User) // Join với bảng User để lấy tên
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new AdminOrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}