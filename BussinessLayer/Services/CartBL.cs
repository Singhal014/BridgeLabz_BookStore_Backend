using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class CartBL : ICartBL
    {
        private readonly ICartRL _cartRL;
        private readonly ILogger<CartBL> _logger;

        public CartBL(ICartRL cartRL, ILogger<CartBL> logger)
        {
            _cartRL = cartRL;
            _logger = logger;
        }

        public async Task<bool> AddToCartAsync(int userId, int bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Adding book {BookId} to cart for user {UserId}", bookId, userId);

                var cartItem = await _cartRL.GetCartItemAsync(userId, bookId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    bool isUpdated = await _cartRL.UpdateCartAsync(cartItem);
                    _logger.LogInformation("Updated quantity of book {BookId} in cart for user {UserId}. New quantity: {Quantity}", bookId, userId, cartItem.Quantity);
                    return isUpdated;
                }
                else
                {
                    var newCartItem = new CartEntity
                    {
                        UserId = userId,
                        BookId = bookId,
                        Quantity = quantity,
                        AddedDate = DateTime.Now,
                        IsRemoved = false,
                        IsOrdered = false
                    };
                    bool isAdded = await _cartRL.AddNewCartItemAsync(newCartItem);
                    _logger.LogInformation("Book {BookId} added to cart for user {UserId} with quantity {Quantity}", bookId, userId, quantity);
                    return isAdded;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AddToCartAsync for UserId {UserId}, BookId {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<bool> RemoveBookFromCartAsync(int userId, int bookId)
        {
            try
            {
                _logger.LogInformation("Removing book {BookId} from cart for user {UserId}", bookId, userId);

                var cartItem = await _cartRL.GetCartItemAsync(userId, bookId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Attempted to remove non-existing book {BookId} from cart for user {UserId}", bookId, userId);
                    return false;
                }

                return await _cartRL.RemoveBookFromCartAsync(cartItem.CartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RemoveBookFromCartAsync for UserId {UserId}, BookId {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<List<CartResponseModel>> GetCartItemsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching cart items for user {UserId}", userId);

                var cartItems = await _cartRL.GetCartItemsAsync(userId);
                _logger.LogInformation("User {UserId} has {CartItemCount} items in the cart", userId, cartItems.Count);

                return cartItems.Select(item => new CartResponseModel
                {
                    CartId = item.CartId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Title = item.Book.Title,
                    AuthorName = item.Book.AuthorName,
                    Price = item.Book.Price,
                    Image = item.Book.Image,
                    TotalPrice = item.Quantity * item.Book.Price
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetCartItemsAsync for UserId {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateCartAsync(int userId, int bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Updating cart for user {UserId} - Book {BookId}, New Quantity: {Quantity}", userId, bookId, quantity);

                var cartItem = await _cartRL.GetCartItemAsync(userId, bookId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cannot update non-existing book {BookId} in cart for user {UserId}", bookId, userId);
                    return false;
                }

                cartItem.Quantity = quantity;
                return await _cartRL.UpdateCartAsync(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateCartAsync for UserId {UserId}, BookId {BookId}", userId, bookId);
                throw;
            }
        }
    }
}
