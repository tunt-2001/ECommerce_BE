namespace ECommerce.Application.Features.Orders.Queries.GetOrdersForAdmin;

public class AdminOrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty; // Lấy từ ApplicationUser
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}