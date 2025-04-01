using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RepoLayer.Services
{
    public class WishlistRL : IWishlistRL
    {
        private readonly ApplicationDbContext _context;

        public WishlistRL(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddToWishlist(int userId, int bookId, int quantity)
        {
            var wishlistItem = _context.Wishlists.FirstOrDefault(w => w.UserId == userId && w.BookId == bookId);

            if (wishlistItem == null)
            {
                var newWishlistItem = new WishlistEntity
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                _context.Wishlists.Add(newWishlistItem);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public bool RemoveFromWishlist(int wishlistId)
        {
            var wishlistItem = _context.Wishlists.FirstOrDefault(w => w.WishlistId == wishlistId);
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public List<WishlistResponseModel> GetWishlistItems(int userId)
        {
            var wishlistItems = _context.Wishlists
                .Include(w => w.Book)
                .Where(w => w.UserId == userId)
                .ToList();

            var wishlistResponseList = wishlistItems.Select(item => new WishlistResponseModel
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

            return wishlistResponseList;
        }

        public bool MoveToCart(int userId, int wishlistId)
        {
            var wishlistItem = _context.Wishlists.FirstOrDefault(w => w.WishlistId == wishlistId);
            if (wishlistItem != null)
            {
                var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.BookId == wishlistItem.BookId && !c.IsOrdered);

                if (cartItem != null)
                {
                    cartItem.Quantity += wishlistItem.Quantity;
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
                    _context.Carts.Add(newCartItem);
                }

                _context.Wishlists.Remove(wishlistItem);
                return _context.SaveChanges() > 0;
            }
            return false;
        }
    }
}
