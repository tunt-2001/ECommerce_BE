using ECommerce.Application.Features.Users.Queries;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Roles = await _userManager.GetRolesAsync(user)
        };
    }
}