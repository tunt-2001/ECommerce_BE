using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationPersistenceService _service;
        public NotificationsController(INotificationPersistenceService service) => _service = service;

        [HttpGet()]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _service.GetAllAsync(userId));
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] List<int> ids)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.MarkNotificationsAsReadAsync(userId, ids);
            return NoContent();
        }
    }
}
