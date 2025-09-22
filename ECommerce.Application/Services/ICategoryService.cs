using ECommerce.Domain.Entities;
public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category> CreateAsync(string name);
    Task<bool> UpdateAsync(int id, string name);
    Task<bool> DeleteAsync(int id);
}