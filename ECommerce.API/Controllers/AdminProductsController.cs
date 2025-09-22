using ECommerce.Application;
using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<AdminProductsController> _logger;

    public AdminProductsController(IProductService productService, ILogger<AdminProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterParameters parameters)
    {
        _logger.LogInformation("Admin fetching all products with filters.");
        var result = await _productService.GetAllAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        _logger.LogInformation("Admin fetching product by ID: {ProductId}", id);
        var product = await _productService.GetByIdAsync(id);
        return product != null ? Ok(product) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        _logger.LogInformation("Admin attempting to create new product: {ProductName}", productDto.Name);
        var product = await _productService.CreateAsync(productDto);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        if (id != productDto.Id) return BadRequest("ID mismatch.");

        _logger.LogInformation("Admin attempting to update product ID: {ProductId}", id);
        var result = await _productService.UpdateAsync(id, productDto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Admin attempting to delete product ID: {ProductId}", id);
        var result = await _productService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}