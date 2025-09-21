namespace ECommerce.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;  
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered
    public string PaymentMethod { get; set; } = string.Empty;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}