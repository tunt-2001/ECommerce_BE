using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> CreateAsync(UserCreateDto dto)
    {
        var userByUsername = await _userManager.FindByNameAsync(dto.UserName);
        if (userByUsername != null)
            return IdentityResult.Failed(new IdentityError { Description = "Username already exists." });

        var userByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (userByEmail != null)
            return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            CreationTime = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded && dto.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, dto.Roles);
        }
        return result;
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        // (Thực tế) Thêm logic kiểm tra xem user có phải là admin cuối cùng không, không cho xóa.

        return await _userManager.DeleteAsync(user);
    }

    public async Task<List<UserListDto>> GetAllAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
        var userDtos = new List<UserListDto>();
        foreach (var user in users)
        {
            userDtos.Add(new UserListDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                FullName = user.FullName,
                Email = user.Email!,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }
        return userDtos;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => new RoleDto { Name = r.Name! })
            .ToListAsync();
    }

    public async Task<UserListDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        return new UserListDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            FullName = user.FullName,
            Email = user.Email!,
            Roles = await _userManager.GetRolesAsync(user)
        };
    }

    public async Task<IdentityResult> UpdateAsync(UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        // Cập nhật thông tin cơ bản
        user.FullName = dto.FullName;
        await _userManager.SetEmailAsync(user, dto.Email);
        await _userManager.SetUserNameAsync(user, dto.UserName);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded) return updateResult;

        // Cập nhật mật khẩu nếu được cung cấp
        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!passwordResult.Succeeded) return passwordResult;
        }

        // Cập nhật Roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!resultRemove.Succeeded) return resultRemove;

        var resultAdd = await _userManager.AddToRolesAsync(user, dto.Roles);
        return resultAdd;
    }
}