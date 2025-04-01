using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Services
{
    public class CartBL : ICartBL
    {
        private readonly ICartRL _cartRL;

        public CartBL(ICartRL cartRL)
        {
            _cartRL = cartRL;
        }

        public bool AddToCart(int userId, int bookId, int quantity)
        {
            var cartItem = _cartRL.GetCartItem(userId, bookId);

            if (cartItem != null)
            {
                // Increase quantity of the book in the cart (not global book quantity)
                cartItem.Quantity += quantity;
                return _cartRL.UpdateCart(cartItem);
            }
            else
            {
                var newCartItem = new CartEntity
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now,
                    IsOrdered = false,
                    IsRemoved = false
                };
                return _cartRL.AddNewCartItem(newCartItem);
            }
        }

        public bool RemoveBookFromCart(int userId, int bookId)
        {
            var cartItem = _cartRL.GetCartItem(userId, bookId);

            if (cartItem != null)
            {
                return _cartRL.RemoveBookFromCart(cartItem.CartId);
            }
            return false;
        }

        public List<CartResponseModel> GetCartItems(int userId)
        {
            var cartItems = _cartRL.GetCartItems(userId);

            return cartItems.Select(item => new CartResponseModel
            {
                CartId = item.CartId,
                UserId = item.UserId,
                BookId = item.BookId,
                Quantity = item.Quantity,
                TotalPrice = item.Quantity * item.Book.Price,
                Title = item.Book.Title,
                AuthorName = item.Book.AuthorName,
                Image = item.Book.Image,
                Price = item.Book.Price
            }).ToList();
        }

        public bool UpdateCart(int userId, int bookId, int quantity)
        {
            var cartItem = _cartRL.GetCartItem(userId, bookId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity; // Update quantity of the book in cart
                return _cartRL.UpdateCart(cartItem);
            }
            return false;
        }

        public bool PlaceOrder(int userId)
        {
            var cartItems = _cartRL.GetCartItems(userId);

            foreach (var item in cartItems)
            {
                var book = _cartRL.GetBookById(item.BookId);
                if (book != null && book.Quantity >= item.Quantity)
                {
                    book.Quantity -= item.Quantity; // Deduct book quantity
                    item.IsOrdered = true;
                }
                else
                {
                    return false;
                }
            }
            return _cartRL.UpdateCartItems(cartItems); // Update cart status after placing order
        }
    }
}
