using BookStore.Controllers;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Tests.Controllers
{
    public class CartControllerTests
    {
        private Mock<ICartBL> cartBL;
        private Mock<ILogger<CartController>> logger;
        private CartController controller;

        [SetUp]
        public void Setup()
        {
            cartBL = new Mock<ICartBL>();
            logger = new Mock<ILogger<CartController>>();
            controller = new CartController(cartBL.Object, logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Test]
        public async Task AddToCart()
        {
            var model = new CartModel { BookId = 1, Quantity = 2 };
            cartBL.Setup(x => x.AddToCartAsync(1, model.BookId, model.Quantity)).ReturnsAsync(true);

            var result = await controller.AddToCart(model) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var data = result.Value as ResponseModel<string>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Book added to cart successfully.", data.Message);
        }

        [Test]
        public async Task GetCartItems()
        {
            var items = new List<CartResponseModel>
            {
                new CartResponseModel { BookId = 1, Quantity = 2 }
            };

            cartBL.Setup(x => x.GetCartItemsAsync(1)).ReturnsAsync(items);

            var result = await controller.GetCartItems() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var data = result.Value as ResponseModel<List<CartResponseModel>>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Cart items retrieved successfully.", data.Message);
        }

        [Test]
        public async Task UpdateCart()
        {
            int cartItemId = 1;
            int newQuantity = 3;

            cartBL.Setup(x => x.UpdateCartAsync(1, cartItemId, newQuantity)).ReturnsAsync(true);

            var result = await controller.UpdateCart(cartItemId, newQuantity) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var data = result.Value as ResponseModel<string>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Book quantity in the cart updated successfully.", data.Message);
        }

        [Test]
        public async Task RemoveBookFromCart()
        {
            int cartItemId = 1;

            cartBL.Setup(x => x.RemoveBookFromCartAsync(1, cartItemId)).ReturnsAsync(true);

            var result = await controller.RemoveBookFromCart(cartItemId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var data = result.Value as ResponseModel<string>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Book removed from cart successfully.", data.Message);
        }


        

        [Test]
        public async Task GetCartItems_Fail()
        {
            cartBL.Setup(x => x.GetCartItemsAsync(1)).ReturnsAsync((List<CartResponseModel>)null);

            var result = await controller.GetCartItems() as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            var data = result.Value as ResponseModel<string>;
            Assert.IsNotNull(data);
            Assert.IsFalse(data.Success);
            Assert.AreEqual("No items found in cart.", data.Message);
        }

       

        
    }
}
