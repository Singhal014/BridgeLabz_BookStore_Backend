using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class WishlistRL : IWishlistRL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistRL> _logger;

        public WishlistRL(ApplicationDbContext context, ILogger<WishlistRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddToWishlistAsync(int userId, int bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Adding BookId {BookId} to wishlist for UserId {UserId}", bookId, userId);

                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

                if (wishlistItem == null)
                {
                    var newWishlistItem = new WishlistEntity
                    {
                        UserId = userId,
                        BookId = bookId,
                        Quantity = quantity,
                        AddedDate = DateTime.Now
                    };

                    await _context.Wishlists.AddAsync(newWishlistItem);
                    var result = await _context.SaveChangesAsync() > 0;

                    if (result)
                        _logger.LogInformation("BookId {BookId} added to wishlist for UserId {UserId}", bookId, userId);
                    else
                        _logger.LogWarning("Failed to add BookId {BookId} to wishlist for UserId {UserId}", bookId, userId);

                    return result;
                }

                _logger.LogWarning("BookId {BookId} already exists in wishlist for UserId {UserId}", bookId, userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding to wishlist for UserId {UserId}, BookId {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                _logger.LogInformation("Removing WishlistId {WishlistId} from wishlist", wishlistId);

                var wishlistItem = await _context.Wishlists.FirstOrDefaultAsync(w => w.WishlistId == wishlistId);

                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                    var result = await _context.SaveChangesAsync() > 0;

                    if (result)
                        _logger.LogInformation("WishlistId {WishlistId} removed successfully", wishlistId);
                    else
                        _logger.LogWarning("Failed to remove WishlistId {WishlistId}", wishlistId);

                    return result;
                }

                _logger.LogWarning("WishlistId {WishlistId} not found", wishlistId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing WishlistId {WishlistId}", wishlistId);
                throw;
            }
        }

        public async Task<List<WishlistResponseModel>> GetWishlistItemsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching wishlist items for UserId {UserId}", userId);

                var wishlistItems = await _context.Wishlists
                    .Include(w => w.Book)
                    .Where(w => w.UserId == userId)
                    .ToListAsync();

                if (!wishlistItems.Any())
                {
                    _logger.LogInformation("No wishlist items found for UserId {UserId}", userId);
                }

                return wishlistItems.Select(item => new WishlistResponseModel
                {
                    WishlistId = item.WishlistId,
                    UserId = item.UserId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Title = item.Book.Title,
                    AuthorName = item.Book.AuthorName,
                    Image = item.Book.Image,
                    Price = item.Book.Price
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching wishlist items for UserId {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MoveToCartAsync(int userId, int wishlistId)
        {
            try
            {
                _logger.LogInformation("Moving WishlistId {WishlistId} to cart for UserId {UserId}", wishlistId, userId);

                var wishlistItem = await _context.Wishlists.FirstOrDefaultAsync(w => w.WishlistId == wishlistId);
                if (wishlistItem != null)
                {
                    var cartItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.UserId == userId &&
                                               c.BookId == wishlistItem.BookId &&
                                               !c.IsOrdered);

                    if (cartItem != null)
                    {
                        cartItem.Quantity += wishlistItem.Quantity;
                        _logger.LogInformation("Updated quantity of BookId {BookId} in cart for UserId {UserId}", wishlistItem.BookId, userId);
                    }
                    else
                    {
                        var newCartItem = new CartEntity
                        {
                            UserId = userId,
                            BookId = wishlistItem.BookId,
                            Quantity = wishlistItem.Quantity,
                            AddedDate = DateTime.Now,
                            IsOrdered = false,
                            IsRemoved = false
                        };
                        await _context.Carts.AddAsync(newCartItem);
                        _logger.LogInformation("Added BookId {BookId} to cart for UserId {UserId}", wishlistItem.BookId, userId);
                    }

                    _context.Wishlists.Remove(wishlistItem);
                    var result = await _context.SaveChangesAsync() > 0;

                    if (result)
                        _logger.LogInformation("Successfully moved WishlistId {WishlistId} to cart for UserId {UserId}", wishlistId, userId);
                    else
                        _logger.LogWarning("Failed to move WishlistId {WishlistId} to cart for UserId {UserId}", wishlistId, userId);

                    return result;
                }

                _logger.LogWarning("WishlistId {WishlistId} not found for UserId {UserId}", wishlistId, userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving WishlistId {WishlistId} to cart for UserId {UserId}", wishlistId, userId);
                throw;
            }
        }
    }
}
