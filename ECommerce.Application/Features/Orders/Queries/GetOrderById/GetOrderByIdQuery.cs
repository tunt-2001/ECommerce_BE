using MediatR;

public class GetOrderByIdQuery : IRequest<OrderDetailDto?>
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty; // Để xác thực đúng chủ đơn hàng
}