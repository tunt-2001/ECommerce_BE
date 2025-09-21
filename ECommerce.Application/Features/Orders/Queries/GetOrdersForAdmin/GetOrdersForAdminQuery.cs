using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrdersForAdmin;

public class GetOrdersForAdminQuery : IRequest<List<AdminOrderDto>>
{
    // Có thể thêm các thuộc tính để lọc/phân trang sau
}