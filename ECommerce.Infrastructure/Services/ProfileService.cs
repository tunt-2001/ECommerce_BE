using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto passwords)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.ChangePasswordAsync(user, passwords.CurrentPassword, passwords.NewPassword);
    }

    public async Task<ProfileDto?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return new ProfileDto
        {
            FullName = user.FullName,
            Email = user.Email!,
            UserName = user.UserName!
        };
    }

    public async Task<IdentityResult> UpdateProfileAsync(string userId, ProfileDto profile)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        user.FullName = profile.FullName;
        // Cần logic kiểm tra trùng lặp nếu cho phép sửa Email/UserName
        await _userManager.SetEmailAsync(user, profile.Email);
        await _userManager.SetUserNameAsync(user, profile.UserName);

        return await _userManager.UpdateAsync(user);
    }
}