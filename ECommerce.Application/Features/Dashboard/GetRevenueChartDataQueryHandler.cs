using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Dashboard
{
    public class GetRevenueChartDataQueryHandler : IRequestHandler<GetRevenueChartDataQuery, List<RevenueByDateDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetRevenueChartDataQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<List<RevenueByDateDto>> Handle(GetRevenueChartDataQuery request, CancellationToken cancellationToken)
        {
            // BƯỚC 1: Thực thi phần có thể dịch được trên SQL Server
            var revenueDataFromDb = await _context.Orders
                .Where(o => o.OrderDate >= request.StartDate && o.OrderDate <= request.EndDate && o.Status != "Canceled")
                .GroupBy(o => o.OrderDate.Date) // Vẫn group theo ngày ở SQL
                .Select(g => new
                {
                    // Lấy về kiểu DateTime, không chuyển thành chuỗi
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(r => r.Date)
                .ToListAsync(cancellationToken); // <-- Kéo dữ liệu về bộ nhớ của ứng dụng

            // BƯỚC 2: Thực hiện phần không dịch được (formatting) trên client (trong bộ nhớ)
            var result = revenueDataFromDb
                .Select(r => new RevenueByDateDto
                {
                    // Bây giờ .ToString() được thực thi bởi C#, không phải SQL
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Revenue = r.Revenue
                })
                .ToList();

            return result;
        }
    }
}
