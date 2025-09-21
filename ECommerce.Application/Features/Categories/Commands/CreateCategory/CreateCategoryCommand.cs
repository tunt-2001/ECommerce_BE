using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

// Command này yêu cầu trả về đối tượng Category vừa được tạo
public class CreateCategoryCommand : IRequest<Category>
{
    public string Name { get; set; } = string.Empty;
}