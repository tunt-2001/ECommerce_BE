using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderCommand : IRequest<int> // Trả về OrderId
    {
        public string UserId { get; set; } = string.Empty; // Sẽ được gán trong controller
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
