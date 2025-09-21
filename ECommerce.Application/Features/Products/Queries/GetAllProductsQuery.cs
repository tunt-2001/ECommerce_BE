using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Products.Queries;

/// <summary>
/// Đại diện cho một yêu cầu để lấy danh sách tất cả sản phẩm.
/// IRequest<List<GetAllProductsDto>> có nghĩa là khi xử lý xong, nó sẽ trả về
/// một danh sách các đối tượng GetAllProductsDto.
/// </summary>
public class GetAllProductsQuery : IRequest<List<GetAllProductsDto>>
{
    // Query này không cần tham số đầu vào, vì chúng ta đang lấy tất cả.
    // Trong tương lai có thể thêm các tham số để phân trang, lọc...
}