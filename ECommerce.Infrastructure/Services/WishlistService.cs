using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IApplicationDbContext _context;
        public WishlistService(IApplicationDbContext context) => _context = context;

        public async Task<List<int>> GetWishlistProductIdsAsync(string userId)
        {
            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.ProductId)
                .ToListAsync();
        }

        public async Task<bool> ToggleWishlistItemAsync(string userId, int productId)
        {
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem != null) // Nếu đã có -> Xóa
            {
                _context.WishlistItems.Remove(existingItem);
                await _context.SaveChangesAsync(default);
                return false; // Trả về false để báo là đã xóa
            }
            else // Nếu chưa có -> Thêm
            {
                var newItem = new WishlistItem { UserId = userId, ProductId = productId };
                _context.WishlistItems.Add(newItem);
                await _context.SaveChangesAsync(default);
                return true; // Trả về true để báo là đã thêm
            }
        }
    }
}
