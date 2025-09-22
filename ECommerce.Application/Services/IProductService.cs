using ECommerce.Application;
using ECommerce.Domain.Entities;
public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<PagedResult<ProductDto>> GetAllAsync(ProductFilterParameters parameters);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductDto productDto);
    Task<bool> UpdateAsync(int id, ProductDto productDto);
    Task<bool> DeleteAsync(int id);
}