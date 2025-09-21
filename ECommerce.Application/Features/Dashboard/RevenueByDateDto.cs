using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Dashboard
{
    public class RevenueByDateDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
