using ECommerce.Application;
using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous] 
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterParameters parameters)
    {
        var result = await _productService.GetAllAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]    
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product != null ? Ok(product) : NotFound();
    }
}