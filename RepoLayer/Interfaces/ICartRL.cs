using RepoLayer.Entity;
using System.Collections.Generic;

namespace RepoLayer.Interfaces
{
    public interface ICartRL
    {
        CartEntity GetCartItem(int userId, int bookId);
        CartEntity GetCartItemById(int cartId);
        bool AddNewCartItem(CartEntity cartItem);
        bool RemoveBookFromCart(int cartId);
        List<CartEntity> GetCartItems(int userId);
        bool UpdateCart(CartEntity cartItem); 
        bool UpdateCartItems(List<CartEntity> cartItems); 
        BookEntity GetBookById(int bookId); 
        bool UpdateBookQuantity(BookEntity book); 
    }
}
