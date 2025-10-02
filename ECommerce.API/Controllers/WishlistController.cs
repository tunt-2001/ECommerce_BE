using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService) => _wishlistService = wishlistService;

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _wishlistService.GetWishlistProductIdsAsync(userId));
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> ToggleWishlistItem(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdded = await _wishlistService.ToggleWishlistItemAsync(userId, productId);
            return Ok(new { isAdded });
        }
    }
}
