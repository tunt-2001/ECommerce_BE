using ECommerce.Application;
using ECommerce.Application.Features.Orders.Queries.GetOrdersForUser;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize] // Bảo vệ toàn bộ, chỉ người đăng nhập mới được thao tác
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        // Lấy UserId từ token để đảm bảo an toàn
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        command.UserId = userId;

        var orderId = await _mediator.Send(command);
        return Ok(new { OrderId = orderId });
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetOrdersForUserQuery { UserId = userId };
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }
}