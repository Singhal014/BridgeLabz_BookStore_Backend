using ModelLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.Interfaces
{
    public interface ICartBL
    {
        bool AddToCart(int userId, int bookId, int quantity);
        bool RemoveBookFromCart(int userId, int bookId);
        List<CartResponseModel> GetCartItems(int userId);
        bool UpdateCart(int userId, int bookId, int quantity);
        bool PlaceOrder(int userId);
    }
}
