using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<GetAllProductsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllProductsDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Truy vấn database, join với bảng Categories để lấy tên danh mục
        var products = await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name) // Sắp xếp theo tên
                                  // Dùng .Select() để "biến hình" từ Entity Product sang GetAllProductsDto
            .Select(p => new GetAllProductsDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category.Name, // Lấy tên từ bảng đã join
                ImageUrl = p.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return products;
    }
}