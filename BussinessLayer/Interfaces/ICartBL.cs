using ModelLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICartBL
    {
        Task<bool> AddToCartAsync(int userId, int bookId, int quantity);
        Task<bool> RemoveBookFromCartAsync(int userId, int bookId);
        Task<List<CartResponseModel>> GetCartItemsAsync(int userId);
        Task<bool> UpdateCartAsync(int userId, int bookId, int quantity);
    }
}