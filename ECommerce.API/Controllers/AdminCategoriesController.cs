using ECommerce.Application.Features.Categories.Commands.CreateCategory;
using ECommerce.Application.Features.Categories.Commands.DeleteCategory;
using ECommerce.Application.Features.Categories.Commands.UpdateCategory;
using ECommerce.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous] // Cho phép cả người dùng thường xem danh sách danh mục
    public async Task<IActionResult> GetCategories()
    {
        var query = new GetAllCategoriesQuery();
        return Ok(await _mediator.Send(query));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var createdCategory = await _mediator.Send(command);
        return Ok(createdCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL and body must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound("Category not found.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _mediator.Send(command);

        // Nếu handler trả về false, có thể là không tìm thấy hoặc do còn sản phẩm.
        // Trả về một lỗi chung chung hơn cho client.
        if (!result)
        {
            return BadRequest("Could not delete category. It might not exist or may contain products.");
        }

        return NoContent();
    }
}