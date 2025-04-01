using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistBL _wishlistBL;

        public WishlistController(IWishlistBL wishlistBL)
        {
            _wishlistBL = wishlistBL;
        }

        [HttpPost("add")]
        public IActionResult AddToWishlist([FromBody] WishlistModel model)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            if (_wishlistBL.AddToWishlist(userId, model.BookId, model.Quantity))
                return Ok("Book added to wishlist successfully.");
            return BadRequest("Failed to add to wishlist.");
        }

        [HttpGet("items")]
        public IActionResult GetWishlistItems()
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var items = _wishlistBL.GetWishlistItems(userId);
            if (items == null || items.Count == 0)
                return NotFound("No items found in wishlist.");
            return Ok(items);
        }

        [HttpDelete("remove/{wishlistId}")]
        public IActionResult RemoveFromWishlist(int wishlistId)
        {
            if (_wishlistBL.RemoveFromWishlist(wishlistId))
                return Ok("Book removed from wishlist successfully.");
            return BadRequest("Failed to remove from wishlist.");
        }

        [HttpPost("move-to-cart/{wishlistId}")]
        public IActionResult MoveToCart(int wishlistId)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            if (_wishlistBL.MoveToCart(userId, wishlistId))
                return Ok("Book moved to cart successfully.");
            return BadRequest("Failed to move to cart.");
        }
    }
}
