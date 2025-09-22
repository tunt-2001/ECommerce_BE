using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/profile")]
[Authorize] // Yêu cầu đăng nhập cho tất cả các hành động
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _profileService.GetProfileAsync(userId);
        return profile != null ? Ok(profile) : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto profile)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _profileService.UpdateProfileAsync(userId, profile);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwords)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _profileService.ChangePasswordAsync(userId, passwords);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }
}