using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        // Thêm logic nghiệp vụ: Không cho phép tự xóa chính mình
        // (Bạn cần có cách lấy Id của user đang thực hiện hành động, thường là từ HttpContext)
        // if (currentUser.Id == request.Id) { ... }

        return await _userManager.DeleteAsync(user);
    }
}