using MediatR;

namespace ECommerce.Application.Features.Categories.Queries.GetAllCategories;

// Query này yêu cầu trả về một danh sách các CategoryDto
public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
}