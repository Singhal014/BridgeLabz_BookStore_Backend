using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IOrderRL
    {
        Task<OrderEntity> PlaceOrder(int userId, int bookId, int quantity, string paymentMethod);
        Task<List<OrderEntity>> GetUserOrders(int userId);
        Task<OrderEntity> GetOrderById(int orderId, int userId);
        Task<bool> CancelOrder(int orderId, int userId);
    }
}
