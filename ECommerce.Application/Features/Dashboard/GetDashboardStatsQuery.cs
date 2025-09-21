using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Dashboard
{
    public class GetDashboardStatsQuery : IRequest<StatsDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
