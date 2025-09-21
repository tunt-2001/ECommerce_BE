using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersForUser;

public class GetOrdersForUserQuery : IRequest<List<UserOrderDto>>
{
    public string UserId { get; set; } = string.Empty;
}