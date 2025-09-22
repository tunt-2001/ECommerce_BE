using ECommerce.Application.DTOs; // Nơi chứa DTO cho Admin
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IAdminOrderService _adminOrderService;
    private readonly ILogger<AdminOrdersController> _logger;

    public AdminOrdersController(IAdminOrderService adminOrderService, ILogger<AdminOrdersController> logger)
    {
        _adminOrderService = adminOrderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        _logger.LogInformation("Admin fetching all orders.");
        var orders = await _adminOrderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        _logger.LogInformation("Admin fetching details for Order ID: {OrderId}", id);
        var order = await _adminOrderService.GetByIdAsync(id);
        return order != null ? Ok(order) : NotFound();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequestDto request)
    {
        _logger.LogInformation("Admin attempting to update status for Order ID: {OrderId} to {NewStatus}", id, request.NewStatus);
        var result = await _adminOrderService.UpdateStatusAsync(id, request.NewStatus);

        if (result)
        {
            _logger.LogInformation("Successfully updated status for Order ID: {OrderId}", id);
            return NoContent();
        }

        _logger.LogWarning("Failed to update status for Order ID: {OrderId}. Order not found.", id);
        return NotFound("Order not found.");
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _adminOrderService.CancelOrderAsync(id);
        return result ? Ok() : BadRequest("Could not cancel order.");
    }
}

public class UpdateStatusRequestDto
{
    public string NewStatus { get; set; } = string.Empty;
}