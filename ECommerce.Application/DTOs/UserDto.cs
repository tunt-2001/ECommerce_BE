using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO dùng để hiển thị thông tin tóm tắt của người dùng trong danh sách.
/// </summary>
public class UserListDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}


/// <summary>
/// DTO dùng để nhận dữ liệu từ request body khi Admin tạo một người dùng mới.
/// </summary>
public class UserCreateDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = new List<string>();
}


/// <summary>
/// DTO dùng để nhận dữ liệu từ request body khi Admin cập nhật thông tin người dùng.
/// </summary>
public class UserUpdateDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Mật khẩu là tùy chọn. Nếu được cung cấp, nó sẽ được cập nhật.
    // Nếu để trống, mật khẩu cũ sẽ được giữ nguyên.
    public string? Password { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}

/// <summary>
/// DTO dùng để hiển thị danh sách các Role có trong hệ thống.
/// </summary>
public class RoleDto
{
    public string Name { get; set; } = string.Empty;
}