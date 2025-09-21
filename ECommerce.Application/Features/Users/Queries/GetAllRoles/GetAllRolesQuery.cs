using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Users.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<List<RoleDto>> { }
}
