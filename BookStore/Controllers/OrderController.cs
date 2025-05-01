using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BookStore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderBL _orderBL;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderBL orderBL, ILogger<OrderController> logger)
        {
            _orderBL = orderBL;
            _logger = logger;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder(int cartId, string paymentMethod)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                var order = await _orderBL.PlaceOrder(userId, cartId, paymentMethod);
                return Ok(new { Success = true, Message = "Order placed", Data = order });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                var orders = await _orderBL.GetUserOrders(userId);
                return Ok(new { Success = true, Message = "Orders retrieved", Data = orders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                var order = await _orderBL.GetOrderById(orderId, userId);
                if (order == null) return NotFound(new { Success = false, Message = "Order not found" });
                return Ok(new { Success = true, Message = "Order retrieved", Data = order });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("Id").Value);
                var result = await _orderBL.CancelOrder(orderId, userId);
                return result ? Ok(new { Success = true, Message = "Order cancelled" })
                             : BadRequest(new { Success = false, Message = "Cannot cancel order" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}