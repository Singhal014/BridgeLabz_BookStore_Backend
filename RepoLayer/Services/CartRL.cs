using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class CartRL : ICartRL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartRL> _logger;

        public CartRL(ApplicationDbContext context, ILogger<CartRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartEntity> GetCartItemAsync(int userId, int bookId)
        {
            try
            {
                return await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId && !c.IsOrdered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItemAsync");
                throw;
            }
        }

        public async Task<CartEntity> GetCartItemByIdAsync(int cartId)
        {
            try
            {
                return await _context.Carts
                    .FirstOrDefaultAsync(c => c.CartId == cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItemByIdAsync");
                throw;
            }
        }

        public async Task<bool> AddNewCartItemAsync(CartEntity cartItem)
        {
            try
            {
                _logger.LogInformation("Adding new cart item for User ID {UserId}, Book ID {BookId}", cartItem.UserId, cartItem.BookId);
                await _context.Carts.AddAsync(cartItem);
                var result = await _context.SaveChangesAsync() > 0;
                if (result)
                    _logger.LogInformation("Cart item added successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddNewCartItemAsync");
                throw;
            }
        }

        public async Task<bool> RemoveBookFromCartAsync(int cartId)
        {
            try
            {
                _logger.LogInformation("Removing cart item with Cart ID {CartId}", cartId);
                var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cartItem != null)
                {
                    _context.Carts.Remove(cartItem);
                    var result = await _context.SaveChangesAsync() > 0;
                    if (result)
                        _logger.LogInformation("Cart item removed successfully.");
                    return result;
                }

                _logger.LogWarning("Cart item with ID {CartId} not found.", cartId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveBookFromCartAsync");
                throw;
            }
        }

        public async Task<List<CartEntity>> GetCartItemsAsync(int userId)
        {
            try
            {
                return await _context.Carts
                    .Where(c => c.UserId == userId && !c.IsOrdered)
                    .Include(c => c.Book)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItemsAsync");
                throw;
            }
        }

        public async Task<bool> UpdateCartAsync(CartEntity cartItem)
        {
            try
            {
                _logger.LogInformation("Updating cart item with Cart ID {CartId}", cartItem.CartId);
                var existingItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

                if (existingItem != null)
                {
                    existingItem.Quantity = cartItem.Quantity;
                    var result = await _context.SaveChangesAsync() > 0;
                    if (result)
                        _logger.LogInformation("Cart item updated successfully.");
                    return result;
                }

                _logger.LogWarning("Cart item with ID {CartId} not found.", cartItem.CartId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCartAsync");
                throw;
            }
        }

        public async Task<bool> UpdateCartItemsAsync(List<CartEntity> cartItems)
        {
            try
            {
                _logger.LogInformation("Updating multiple cart items.");
                foreach (var cartItem in cartItems)
                {
                    var existingItem = await _context.Carts
                        .FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity = cartItem.Quantity;
                    }
                    else
                    {
                        _logger.LogWarning("Cart item with ID {CartId} not found.", cartItem.CartId);
                        return false;
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;
                if (result)
                    _logger.LogInformation("All cart items updated successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCartItemsAsync");
                throw;
            }
        }

        public async Task<BookEntity> GetBookByIdAsync(int bookId)
        {
            try
            {
                return await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBookByIdAsync");
                throw;
            }
        }

        public async Task<bool> UpdateBookQuantityAsync(BookEntity book)
        {
            try
            {
                _logger.LogInformation("Updating book quantity for Book ID {BookId}", book.Id);
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == book.Id);

                if (existingBook != null)
                {
                    existingBook.Quantity = book.Quantity;
                    var result = await _context.SaveChangesAsync() > 0;
                    if (result)
                        _logger.LogInformation("Book quantity updated successfully.");
                    return result;
                }

                _logger.LogWarning("Book with ID {BookId} not found.", book.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateBookQuantityAsync");
                throw;
            }
        }
    }
}
