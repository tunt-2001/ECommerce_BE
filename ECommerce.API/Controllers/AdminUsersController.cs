using ECommerce.Application.DTOs; // Giả sử các DTO của User nằm ở đây
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AdminUsersController> _logger;

    public AdminUsersController(IUserService userService, ILogger<AdminUsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        _logger.LogInformation("Admin fetching all users.");
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        _logger.LogInformation("Admin fetching all available roles.");
        var roles = await _userService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        _logger.LogInformation("Admin fetching user by ID: {UserId}", id);
        var user = await _userService.GetByIdAsync(id);
        return user != null ? Ok(user) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        _logger.LogInformation("Admin attempting to create a new user with username: {Username}", userDto.UserName);
        var result = await _userService.CreateAsync(userDto);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User created successfully." });
        }
        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest("User ID mismatch.");
        }
        _logger.LogInformation("Admin attempting to update user with ID: {UserId}", id);
        var result = await _userService.UpdateAsync(userDto);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        _logger.LogInformation("Admin attempting to delete user with ID: {UserId}", id);
        var result = await _userService.DeleteAsync(id);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}