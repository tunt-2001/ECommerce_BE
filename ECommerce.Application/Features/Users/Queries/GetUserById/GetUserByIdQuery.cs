using ECommerce.Application.Features.Users.Queries;
using MediatR;

public class GetUserByIdQuery : IRequest<UserDto?> { public string Id { get; set; } = string.Empty; }