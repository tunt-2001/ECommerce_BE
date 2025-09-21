using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<bool>
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; } = string.Empty;
}