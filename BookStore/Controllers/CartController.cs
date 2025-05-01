using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BookStore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBL _cartBL;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartBL cartBL, ILogger<CartController> logger)
        {
            _cartBL = cartBL;
            _logger = logger;
        }

        /// <summary>
        /// Adds a book to the user's cart with a specified quantity.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartModel model)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation($"User {userId} is adding book {model.BookId} with quantity {model.Quantity} to the cart.");

                bool result = await _cartBL.AddToCartAsync(userId, model.BookId, model.Quantity);

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book added to cart successfully." : "Failed to add book to cart.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding book to cart: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves all cart items for the authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation($"Fetching cart items for user {userId}.");

                var items = await _cartBL.GetCartItemsAsync(userId);

                if (items == null || items.Count == 0)
                {
                    _logger.LogWarning($"No items found in cart for user {userId}.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "No items found in cart.",
                        Data = null
                    });
                }

                return Ok(new ResponseModel<List<CartResponseModel>>
                {
                    Success = true,
                    Message = "Cart items retrieved successfully.",
                    Data = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving cart items: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Updates the quantity of a specific book in the user's cart.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateCart(int bookId, int quantity)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation($"Updating cart: User {userId}, Book {bookId}, New Quantity {quantity}.");

                bool result = await _cartBL.UpdateCartAsync(userId, bookId, quantity);

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book quantity in the cart updated successfully."
                                    : "Failed to update book quantity in cart.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating cart: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Removes a book from the user's cart.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> RemoveBookFromCart(int bookId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                _logger.LogInformation($"User {userId} is removing book {bookId} from cart.");

                bool result = await _cartBL.RemoveBookFromCartAsync(userId, bookId);

                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Book removed from cart successfully."
                                    : "Failed to remove book from cart.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing book from cart: {ex.Message}");
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