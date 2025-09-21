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

    public class CreateOrderResultDto
    {
        public int OrderId { get; set; }
        public string? QRCodeImageBase64 { get; set; } // Nullable, chỉ có giá trị nếu thanh toán QR
    }

    public class CreateOrderCommand : IRequest<CreateOrderResultDto>
    {
        public string UserId { get; set; } = string.Empty; // Sẽ được gán trong controller
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
