using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.UpdateCategory;

// Command này yêu cầu trả về bool (thành công / thất bại)
public class UpdateCategoryCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}