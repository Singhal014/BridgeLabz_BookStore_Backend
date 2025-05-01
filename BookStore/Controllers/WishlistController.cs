using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistBL _wishlistBL;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistBL wishlistBL, ILogger<WishlistController> logger)
        {
            _wishlistBL = wishlistBL;
            _logger = logger;
        }

        /// <summary>
        /// Adds a book to the user's wishlist with a specified quantity.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistModel model)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Adding book with ID {BookId} to wishlist for user {UserId}.", model.BookId, userId);

                bool result = await _wishlistBL.AddToWishlistAsync(userId, model.BookId, model.Quantity);

                _logger.LogInformation(result ? "Book added successfully." : "Failed to add book to wishlist.");

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book added to wishlist successfully." : "Failed to add to wishlist.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book to wishlist.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves all wishlist items for the authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlistItems()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Retrieving wishlist items for user {UserId}.", userId);

                var items = await _wishlistBL.GetWishlistItemsAsync(userId);

                if (items == null || items.Count == 0)
                {
                    _logger.LogWarning("No items found in wishlist for user {UserId}.", userId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "No items found in wishlist.",
                        Data = null
                    });
                }

                _logger.LogInformation("Wishlist items retrieved successfully for user {UserId}.", userId);

                return Ok(new ResponseModel<List<WishlistResponseModel>>
                {
                    Success = true,
                    Message = "Wishlist items retrieved successfully.",
                    Data = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving wishlist items.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        ///  Removes a book from the wishlist using its wishlistId.
        /// </summary>
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            try
            {
                _logger.LogInformation("Removing book with wishlist ID {WishlistId}.", wishlistId);

                bool result = await _wishlistBL.RemoveFromWishlistAsync(wishlistId);

                _logger.LogInformation(result ? "Book removed successfully." : "Failed to remove book from wishlist.");

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book removed from wishlist successfully." : "Failed to remove from wishlist.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book from wishlist.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Moves a book from the wishlist to the cart for the authenticated user.
        /// </summary>
        [HttpPost("move-to-cart")]
        public async Task<IActionResult> MoveToCart(int wishlistId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation("Moving book with wishlist ID {WishlistId} to cart for user {UserId}.", wishlistId, userId);

                bool result = await _wishlistBL.MoveToCartAsync(userId, wishlistId);

                _logger.LogInformation(result ? "Book moved to cart successfully." : "Failed to move book to cart.");

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book moved to cart successfully." : "Failed to move to cart.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving book to cart.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}