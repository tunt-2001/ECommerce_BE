// File: ECommerce.Application/.../CreateProductCommandHandler.cs
using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IApplicationDbContext _context; 

    public CreateProductCommandHandler(IApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product { /* ... */ };

        // Bây giờ truy cập qua interface
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return product;
    }
}