using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs
{
    public class OrderItemRequestDto { public int ProductId { get; set; } public int Quantity { get; set; } }
    public class OrderRequestDto { public string ShippingAddress { get; set; } public string PaymentMethod { get; set; } = string.Empty; public List<OrderItemRequestDto> OrderItems { get; set; } = new(); }
    public class OrderResultDto { public int OrderId { get; set; } public string? QRCodeImageBase64 { get; set; } }
    public class UserOrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class OrderItemDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<OrderItemDetailDto> Items { get; set; } = new();
    }
}
