// File: ECommerce.Application/Features/Users/Commands/CreateUser/CreateUserCommand.cs
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<IdentityResult>
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}