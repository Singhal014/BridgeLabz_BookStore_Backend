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
    public class CartController : ControllerBase
    {
        private readonly ICartBL _cartBL;

        public CartController(ICartBL cartBL)
        {
            _cartBL = cartBL;
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartModel model)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            if (_cartBL.AddToCart(userId, model.BookId, model.Quantity))
                return Ok("Book added to cart successfully.");
            return BadRequest("Failed to add book to cart.");
        }

        [HttpGet("items")]
        public IActionResult GetCartItems()
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var items = _cartBL.GetCartItems(userId);
            if (items == null || items.Count == 0)
                return NotFound("No items found in cart.");
            return Ok(items);
        }

        [HttpPut("update/{bookId}/{quantity}")]
        public IActionResult UpdateCart(int bookId, int quantity)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);

            if (_cartBL.UpdateCart(userId, bookId, quantity))
                return Ok("Book quantity in the cart updated successfully.");

            return BadRequest("Failed to update book quantity in cart.");
        }

        [HttpDelete("remove/{bookId}")]
        public IActionResult RemoveBookFromCart(int bookId)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            if (_cartBL.RemoveBookFromCart(userId, bookId))
                return Ok("Book removed from cart successfully.");
            return BadRequest("Failed to remove book from cart.");
        }

        [HttpPost("place-order")]
        public IActionResult PlaceOrder()
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            if (_cartBL.PlaceOrder(userId))
                return Ok("Order placed successfully.");
            return BadRequest("Failed to place order.");
        }
    }
}
