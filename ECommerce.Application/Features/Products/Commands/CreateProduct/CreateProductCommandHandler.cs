using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore; // Thêm using này để dùng .AnyAsync()
using Microsoft.Extensions.Logging;   // Thêm using này để ghi log
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Products.Commands.CreateProduct; // Thêm namespace chính xác

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IApplicationDbContext context, ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // === BƯỚC 1: VALIDATE DỮ LIỆU ĐẦU VÀO ===
        // Kiểm tra xem CategoryId có tồn tại trong database không.
        // Đây là bước kiểm tra logic nghiệp vụ quan trọng để tránh lỗi khóa ngoại.
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            _logger.LogWarning("Attempted to create a product with a non-existent Category ID: {CategoryId}", request.CategoryId);
            // Ném ra một exception rõ ràng để lớp API có thể bắt và trả về lỗi 400 Bad Request.
            throw new ArgumentException($"Category with ID {request.CategoryId} does not exist.", nameof(request.CategoryId));
        }

        // === BƯỚC 2: TẠO ENTITY MỚI TỪ DỮ LIỆU CỦA COMMAND ===
        // Lấy dữ liệu từ object `request` (chính là CreateProductCommand) và gán vào các thuộc tính của Product.
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId
            // Các thuộc tính khác như Id, CreatedDate... sẽ được database tự động tạo.
        };

        _logger.LogInformation("Creating a new product with Name: {ProductName} and CategoryId: {CategoryId}", product.Name, product.CategoryId);

        // === BƯỚC 3: THÊM VÀ LƯU VÀO DATABASE ===
        try
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Ghi log chi tiết nếu có lỗi từ database (ví dụ: lỗi ràng buộc, kết nối...)
            _logger.LogError(ex, "An error occurred in the database while creating the product.");
            // Ném lại exception để lớp cao hơn xử lý
            throw;
        }

        _logger.LogInformation("Successfully created product with new ID: {ProductId}", product.Id);

        // === BƯỚC 4: TRẢ VỀ ENTITY VỪA ĐƯỢC TẠO ===
        // Trả về đối tượng product đã có Id được gán bởi database.
        return product;
    }
}