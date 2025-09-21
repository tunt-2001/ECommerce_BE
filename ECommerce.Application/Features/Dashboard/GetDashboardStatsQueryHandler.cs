using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Dashboard
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, StatsDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetDashboardStatsQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<StatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // Lọc các đơn hàng trong khoảng thời gian
            var ordersInPeriod = _context.Orders
                .Where(o => o.OrderDate >= request.StartDate && o.OrderDate <= request.EndDate && o.Status != "Canceled");

            var stats = new StatsDto
            {
                TotalRevenue = await ordersInPeriod.SumAsync(o => o.TotalAmount, cancellationToken),
                TotalOrders = await ordersInPeriod.CountAsync(cancellationToken),
                // Đếm user được tạo trong khoảng thời gian
                NewCustomers = await _userManager.Users
                    .CountAsync(u => u.CreationTime >= request.StartDate && u.CreationTime <= request.EndDate, cancellationToken)
            };

            return stats;
        }
    }
    // Lưu ý: Bạn cần thêm thuộc tính `CreationTime` vào `ApplicationUser.cs`
    // public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    // và chạy migration để cập nhật DB.
}
