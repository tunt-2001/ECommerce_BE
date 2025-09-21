using ECommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra nghiệp vụ: Không cho xóa danh mục nếu vẫn còn sản phẩm
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == request.Id, cancellationToken);
        if (hasProducts)
        {
            // Cách 1: Trả về false để báo thất bại
            return false;
            // Cách 2 (Tốt hơn): Ném ra một exception nghiệp vụ để Controller có thể bắt và trả về lỗi 400 Bad Request
            // throw new InvalidOperationException("Cannot delete a category that contains products.");
        }

        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null)
        {
            return false; // Không tìm thấy
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}