using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Users.Commands.DeleteUser;

// Command này trả về IdentityResult để có thể xem lỗi chi tiết
public class DeleteUserCommand : IRequest<IdentityResult>
{
    public string Id { get; set; } = string.Empty;
}