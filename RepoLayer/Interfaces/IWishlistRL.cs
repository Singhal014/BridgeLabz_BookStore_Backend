using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer.Models;

namespace RepoLayer.Interfaces
{
    public interface IWishlistRL
    {
        Task<bool> AddToWishlistAsync(int userId, int bookId, int quantity);
        Task<bool> RemoveFromWishlistAsync(int wishlistId);
        Task<List<WishlistResponseModel>> GetWishlistItemsAsync(int userId);
        Task<bool> MoveToCartAsync(int userId, int wishlistId);
    }
}