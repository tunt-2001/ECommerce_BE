using ECommerce.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ECommerce.Application.Services;

public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(string userId);
    Task<IdentityResult> UpdateProfileAsync(string userId, ProfileDto profile);
    Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto passwords);
}