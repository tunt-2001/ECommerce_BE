using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;
    public CategoryService(IApplicationDbContext context) => _context = context;

    public async Task<Category> CreateAsync(string name)
    {
        var category = new Category { Name = name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(default);
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _context.Products.AnyAsync(p => p.CategoryId == id))
        {
            throw new InvalidOperationException("Cannot delete a category with associated products.");
        }
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;
        _context.Categories.Remove(category);
        return await _context.SaveChangesAsync(default) > 0;
    }

    public async Task<List<Category>> GetAllAsync() => await _context.Categories.OrderBy(c => c.Name).ToListAsync();

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;
        category.Name = name;
        return await _context.SaveChangesAsync(default) > 0;
    }
}