using ECommerce.Application;
using ECommerce.Domain.Entities;
public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<Product> CreateAsync(ProductDto productDto);
    Task<bool> UpdateAsync(int id, ProductDto productDto);
    Task<bool> DeleteAsync(int id);
}