using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface ICartRL
    {
        Task<CartEntity> GetCartItemAsync(int userId, int bookId);
        Task<CartEntity> GetCartItemByIdAsync(int cartId);
        Task<bool> AddNewCartItemAsync(CartEntity cartItem);
        Task<bool> RemoveBookFromCartAsync(int cartId);
        Task<List<CartEntity>> GetCartItemsAsync(int userId);
        Task<bool> UpdateCartAsync(CartEntity cartItem);
        Task<bool> UpdateCartItemsAsync(List<CartEntity> cartItems);
        Task<BookEntity> GetBookByIdAsync(int bookId);
        Task<bool> UpdateBookQuantityAsync(BookEntity book);
    }
}