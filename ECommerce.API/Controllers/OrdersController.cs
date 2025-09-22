using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize] 
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        _logger.LogInformation("User {UserId} is attempting to create an order.", userId);

        try
        {
            var result = await _orderService.CreateOrderAsync(userId, request);
            _logger.LogInformation("Order {OrderId} created successfully for User {UserId}.", result.OrderId, userId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business logic error while creating order for User {UserId}.", userId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating order for User {UserId}.", userId);
            return StatusCode(500, "An internal error occurred. Please try again later.");
        }
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!; 
        _logger.LogInformation("Fetching orders for User {UserId}.", userId);
        var orders = await _orderService.GetMyOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _logger.LogInformation("Fetching order details for Order {OrderId} by User {UserId}.", id, userId);
        var order = await _orderService.GetOrderByIdAsync(id, userId);

        if (order == null)
        {
            _logger.LogWarning("User {UserId} attempted to access order {OrderId} which was not found or does not belong to them.", userId, id);
            return NotFound();
        }

        return Ok(order);
    }
}
