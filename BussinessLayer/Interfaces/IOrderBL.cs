using ModelLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IOrderBL
    {
        Task<OrderResponseModel> PlaceOrder(int userId, int cartId, string paymentMethod);
        Task<List<OrderResponseModel>> GetUserOrders(int userId);
        Task<OrderResponseModel> GetOrderById(int orderId, int userId);
        Task<bool> CancelOrder(int orderId, int userId);
    }
}