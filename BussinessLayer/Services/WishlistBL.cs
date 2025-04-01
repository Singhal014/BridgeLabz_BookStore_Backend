using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Interfaces;
using System.Collections.Generic;

namespace BusinessLayer.Services
{
    public class WishlistBL : IWishlistBL
    {
        private readonly IWishlistRL _wishlistRL;

        public WishlistBL(IWishlistRL wishlistRL)
        {
            _wishlistRL = wishlistRL;
        }

        public bool AddToWishlist(int userId, int bookId, int quantity)
        {
            return _wishlistRL.AddToWishlist(userId, bookId, quantity);
        }

        public bool RemoveFromWishlist(int wishlistId)
        {
            return _wishlistRL.RemoveFromWishlist(wishlistId);
        }

        public List<WishlistResponseModel> GetWishlistItems(int userId)
        {
            return _wishlistRL.GetWishlistItems(userId);
        }

        public bool MoveToCart(int userId, int wishlistId)
        {
            return _wishlistRL.MoveToCart(userId, wishlistId);
        }
    }
}
