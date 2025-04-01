using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace RepoLayer.Services
{
    public class CartRL : ICartRL
    {
        private readonly ApplicationDbContext _context;

        public CartRL(ApplicationDbContext context)
        {
            _context = context;
        }

        public CartEntity GetCartItem(int userId, int bookId)
        {
            return _context.Carts
                .FirstOrDefault(c => c.UserId == userId && c.BookId == bookId && !c.IsOrdered);
        }

        public CartEntity GetCartItemById(int cartId)
        {
            return _context.Carts
                .FirstOrDefault(c => c.CartId == cartId);
        }

        public bool AddNewCartItem(CartEntity cartItem)
        {
            _context.Carts.Add(cartItem);
            return _context.SaveChanges() > 0;
        }

        public bool RemoveBookFromCart(int cartId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public List<CartEntity> GetCartItems(int userId)
        {
            return _context.Carts
                .Where(c => c.UserId == userId && !c.IsOrdered)
                .Include(c => c.Book)
                .ToList();
        }

        public bool UpdateCart(CartEntity cartItem)
        {
            var existingItem = _context.Carts
                .FirstOrDefault(c => c.CartId == cartItem.CartId);

            if (existingItem != null)
            {
                existingItem.Quantity = cartItem.Quantity;
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateCartItems(List<CartEntity> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                var existingItem = _context.Carts
                    .FirstOrDefault(c => c.CartId == cartItem.CartId);

                if (existingItem != null)
                {
                    existingItem.Quantity = cartItem.Quantity;
                }
                else
                {
                    return false;
                }
            }
            return _context.SaveChanges() > 0;
        }

        public BookEntity GetBookById(int bookId)
        {
            return _context.Books
                .FirstOrDefault(b => b.Id == bookId);
        }

        public bool UpdateBookQuantity(BookEntity book)
        {
            var existingBook = _context.Books
                .FirstOrDefault(b => b.Id == book.Id);

            if (existingBook != null)
            {
                existingBook.Quantity = book.Quantity;
                return _context.SaveChanges() > 0;
            }
            return false;
        }
    }
}
