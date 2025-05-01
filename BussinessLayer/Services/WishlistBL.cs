using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class WishlistBL : IWishlistBL
    {
        private readonly IWishlistRL _wishlistRL;
        private readonly ILogger<WishlistBL> _logger;

        public WishlistBL(IWishlistRL wishlistRL, ILogger<WishlistBL> logger)
        {
            _wishlistRL = wishlistRL;
            _logger = logger;
        }

        public async Task<bool> AddToWishlistAsync(int userId, int bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Adding book {BookId} to wishlist for user {UserId} with quantity {Quantity}", bookId, userId, quantity);
                return await _wishlistRL.AddToWishlistAsync(userId, bookId, quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding book {BookId} to wishlist for user {UserId}", bookId, userId);
                throw;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                _logger.LogInformation("Removing item {WishlistId} from wishlist", wishlistId);
                return await _wishlistRL.RemoveFromWishlistAsync(wishlistId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing item {WishlistId} from wishlist", wishlistId);
                throw;
            }
        }

        public async Task<List<WishlistResponseModel>> GetWishlistItemsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching wishlist items for user {UserId}", userId);
                return await _wishlistRL.GetWishlistItemsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching wishlist items for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MoveToCartAsync(int userId, int wishlistId)
        {
            try
            {
                _logger.LogInformation("Moving wishlist item {WishlistId} to cart for user {UserId}", wishlistId, userId);
                return await _wishlistRL.MoveToCartAsync(userId, wishlistId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while moving wishlist item {WishlistId} to cart for user {UserId}", wishlistId, userId);
                throw;
            }
        }
    }
}
