using ECommerce.Application.Features.Users.Commands.CreateUser;
using ECommerce.Application.Features.Users.Commands.DeleteUser;
using ECommerce.Application.Features.Users.Commands.UpdateUser;
using ECommerce.Application.Features.Users.Queries;
using ECommerce.Application.Features.Users.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")] // BẢO VỆ TOÀN BỘ CONTROLLER, CHỈ ADMIN MỚI ĐƯỢC DÙNG
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/admin/users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var query = new GetUsersQuery();
        var users = await _mediator.Send(query);
        return Ok(users);
    }

    // POST: api/admin/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User created successfully." });
        }
        return BadRequest(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var user = await _mediator.Send(query);
        return user != null ? Ok(user) : NotFound();
    }

    [HttpGet("roles")] 
    public async Task<IActionResult> GetAllRoles()
    {
        var query = new GetAllRolesQuery();
        var roles = await _mediator.Send(query);
        return Ok(roles);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest();

        var result = await _mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}