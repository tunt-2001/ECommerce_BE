using ECommerce.Application.Features.Users.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

    public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return await _roleManager.Roles
            .Select(r => new RoleDto { Name = r.Name! })
            .ToListAsync(cancellationToken);
    }
}