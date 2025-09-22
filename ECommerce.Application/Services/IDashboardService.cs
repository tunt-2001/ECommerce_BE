using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public interface IDashboardService
    {
        Task<StatsDto> GetStatsAsync(DateTime start, DateTime end); Task<List<RevenueByDateDto>> GetRevenueChartDataAsync(DateTime start, DateTime end);
    }
}
