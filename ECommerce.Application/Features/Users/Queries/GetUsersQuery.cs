// File: ECommerce.Application/Features/Users/Queries/GetUsersQuery.cs
using MediatR;

namespace ECommerce.Application.Features.Users.Queries;

// Query này không cần tham số đầu vào
public class GetUsersQuery : IRequest<List<UserDto>>
{
}