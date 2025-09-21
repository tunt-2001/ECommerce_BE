using ECommerce.Application;
using ECommerce.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    // GET /api/products - Dùng lại Query của Admin
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var query = new GetAllProductsQuery();
        return Ok(await _mediator.Send(query));
    }

    // GET /api/products/{id} - Endpoint này đã có ở trên
    // Bạn nên tạo Query Handler riêng cho nó và chuyển nó vào đây.
}