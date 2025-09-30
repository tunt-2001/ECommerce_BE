using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin/marketing")]
    [Authorize(Roles = "Admin")]
    public class AdminMarketingController : ControllerBase
    {
        private readonly IAdminMarketingService _marketingService;
        public AdminMarketingController(IAdminMarketingService marketingService) => _marketingService = marketingService;

        [HttpPost("send-newsletter")]
        public async Task<IActionResult> SendNewsletter([FromBody] NewsletterDto dto)
        {
            await _marketingService.SendNewsletterAsync(dto);
            return Ok(new { message = "Newsletter is being sent to all users." });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _marketingService.GetNewsletterHistoryAsync();
            return Ok(history);
        }
    }
}
