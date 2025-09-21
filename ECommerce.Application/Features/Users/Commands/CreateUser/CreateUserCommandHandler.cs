using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra xem UserName đã được sử dụng chưa
        var userByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (userByUsername != null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "Username already exists." });
        }

        // 2. Kiểm tra xem Email đã được sử dụng chưa
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userByEmail != null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email already exists." });
        }

        // 3. Tạo user mới với đầy đủ thông tin
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Gán các quyền được chọn
            if (request.Roles != null && request.Roles.Any())
            {
                await _userManager.AddToRolesAsync(user, request.Roles);
            }
        }

        return result;
    }
}