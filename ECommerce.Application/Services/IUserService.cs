using ECommerce.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Services;

public interface IUserService
{
    Task<List<UserListDto>> GetAllAsync();
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<UserListDto?> GetByIdAsync(string id);
    Task<IdentityResult> CreateAsync(UserCreateDto userDto);
    Task<IdentityResult> UpdateAsync(UserUpdateDto userDto);
    Task<IdentityResult> DeleteAsync(string id);
}