using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoLayer.Entity;
using ModelLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IWishlistBL
    {
        bool AddToWishlist(int userId, int bookId, int quantity);
        bool RemoveFromWishlist(int wishlistId);
        List<WishlistResponseModel> GetWishlistItems(int userId);
        bool MoveToCart(int userId, int wishlistId);
    }
}
