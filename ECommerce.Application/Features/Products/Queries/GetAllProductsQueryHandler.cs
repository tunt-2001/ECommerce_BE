using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<GetAllProductsDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllProductsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<List<GetAllProductsDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Include(p => p.Category) // Join với bảng Category
                .Select(p => new GetAllProductsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryName = p.Category.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
