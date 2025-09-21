using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<IdentityResult>
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}