using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class OrderBL : IOrderBL
    {
        private readonly IOrderRL _orderRL;
        private readonly ICartRL _cartRL;  
        private readonly ILogger<OrderBL> _logger;

        public OrderBL(IOrderRL orderRL, ICartRL cartRL, ILogger<OrderBL> logger)
        {
            _orderRL = orderRL;
            _cartRL = cartRL;  
            _logger = logger;
        }

        public async Task<OrderResponseModel> PlaceOrder(int userId, int cartId, string paymentMethod)
        {
            try
            {
                _logger.LogInformation($"Attempting to place order for user {userId} from cart {cartId}");

                var cartItem = await _cartRL.GetCartItemByIdAsync(cartId);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item {cartId} not found");
                    throw new Exception("Cart item not found");
                }

                if (cartItem.UserId != userId)
                {
                    _logger.LogWarning($"Cart item {cartId} doesn't belong to user {userId}");
                    throw new Exception("Cart item doesn't belong to user");
                }

                // Place the order
                var order = await _orderRL.PlaceOrder(
                    userId: userId,
                    bookId: cartItem.BookId,
                    quantity: cartItem.Quantity,
                    paymentMethod: paymentMethod);

                // Remove from cart
                await _cartRL.RemoveBookFromCartAsync(cartId);

                _logger.LogInformation($"Order {order.OrderId} placed successfully from cart {cartId}");

                return MapToResponseModel(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error placing order from cart {cartId} for user {userId}");
                throw;
            }
        }

        public async Task<List<OrderResponseModel>> GetUserOrders(int userId)
        {
            try
            {
                _logger.LogInformation($"Getting orders for user {userId}");
                var orders = await _orderRL.GetUserOrders(userId);
                return orders.Select(MapToResponseModel).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                throw;
            }
        }

        public async Task<OrderResponseModel> GetOrderById(int orderId, int userId)
        {
            try
            {
                _logger.LogInformation($"Getting order {orderId} for user {userId}");
                var order = await _orderRL.GetOrderById(orderId, userId);

                if (order == null)
                {
                    _logger.LogWarning($"Order {orderId} not found for user {userId}");
                    return null;
                }

                return MapToResponseModel(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {orderId}");
                throw;
            }
        }

        public async Task<bool> CancelOrder(int orderId, int userId)
        {
            try
            {
                _logger.LogInformation($"Attempting to cancel order {orderId} for user {userId}");
                var result = await _orderRL.CancelOrder(orderId, userId);

                if (result)
                {
                    _logger.LogInformation($"Order {orderId} cancelled successfully");
                }
                else
                {
                    _logger.LogWarning($"Failed to cancel order {orderId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {orderId}");
                throw;
            }
        }

        private OrderResponseModel MapToResponseModel(OrderEntity order)
        {
            return new OrderResponseModel
            {
                OrderId = order.OrderId,
                BookId = order.BookId,
                Title = order.Book?.Title,
                AuthorName = order.Book?.AuthorName,
                Quantity = order.Quantity,
                Price = order.Price,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                Image = order.Book?.Image
            };
        }
    }
}