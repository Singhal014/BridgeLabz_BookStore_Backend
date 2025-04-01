using ModelLayer.Models;
using RepoLayer.Entity;
using System.Collections.Generic;

namespace RepoLayer.Interfaces
{
    public interface IWishlistRL
    {
        bool AddToWishlist(int userId, int bookId, int quantity);
        bool RemoveFromWishlist(int wishlistId);
        List<WishlistResponseModel> GetWishlistItems(int userId);
        bool MoveToCart(int userId, int wishlistId);
    }
}
