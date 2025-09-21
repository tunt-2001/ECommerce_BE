using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        // Cập nhật các thông tin cơ bản
        user.FullName = request.FullName;

        // Cập nhật và chuẩn hóa lại Email và UserName
        // Lưu ý: Cần có logic kiểm tra trùng lặp nếu cho phép sửa UserName/Email
        await _userManager.SetEmailAsync(user, request.Email);
        await _userManager.SetUserNameAsync(user, request.UserName);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return updateResult;
        }

        // Cập nhật Roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        // Xóa các role mà user đang có nhưng không có trong danh sách mới
        var rolesToRemove = currentRoles.Except(request.Roles);
        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        // Thêm các role mới mà user chưa có
        var rolesToAdd = request.Roles.Except(currentRoles);
        await _userManager.AddToRolesAsync(user, rolesToAdd);

        return IdentityResult.Success;
    }
}