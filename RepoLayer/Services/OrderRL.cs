using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class OrderRL : IOrderRL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRL> _logger;
        private readonly ICartRL _cartRL;

        public OrderRL(ApplicationDbContext context, ILogger<OrderRL> logger, ICartRL cartRL)
        {
            _context = context;
            _logger = logger;
            _cartRL = cartRL;
        }

        public async Task<OrderEntity> PlaceOrder(int userId, int bookId, int quantity, string paymentMethod)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Book {BookId} not found", bookId);
                    throw new Exception("Book not found");
                }

                var order = new OrderEntity
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    Price = book.Price,
                    TotalPrice = quantity * book.Price,
                    PaymentMethod = paymentMethod,
                    Status = "Placed"
                };

                var cartItem = await _cartRL.GetCartItemAsync(userId, bookId);
                if (cartItem != null)
                {
                    await _cartRL.RemoveBookFromCartAsync(cartItem.CartId);
                }

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order placed successfully for user {UserId}", userId);
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error placing order for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<OrderEntity>> GetUserOrders(int userId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.Book)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user {UserId}", userId);
                throw;
            }
        }

        public async Task<OrderEntity> GetOrderById(int orderId, int userId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Book)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> CancelOrder(int orderId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null || order.Status != "Placed")
                {
                    _logger.LogWarning("Order {OrderId} cannot be cancelled", orderId);
                    return false;
                }

                order.Status = "Cancelled";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                throw;
            }
        }
    }
}
