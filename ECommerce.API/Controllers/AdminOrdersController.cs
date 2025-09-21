using ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using ECommerce.Application.Features.Orders.Queries.GetOrdersForAdmin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var query = new GetOrdersForAdminQuery();
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var command = new UpdateOrderStatusCommand { OrderId = id, NewStatus = dto.NewStatus };
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound("Order not found.");
    }
}

public class UpdateStatusDto
{
    public string NewStatus { get; set; } = string.Empty;
}