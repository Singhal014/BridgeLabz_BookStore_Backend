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

namespace BookStoreTest.ControllerTests
{
    public class WishlistControllerTests
    {
        private Mock<IWishlistBL> _wishlistBL;
        private Mock<ILogger<WishlistController>> _logger;
        private WishlistController _controller;

        [SetUp]
        public void Setup()
        {
            _wishlistBL = new Mock<IWishlistBL>();
            _logger = new Mock<ILogger<WishlistController>>();
            _controller = new WishlistController(_wishlistBL.Object, _logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }


        [Test]
        public async Task AddToWishlist()
        {
            var model = new WishlistModel { BookId = 101, Quantity = 1 };

            _wishlistBL.Setup(x => x.AddToWishlistAsync(1, model.BookId, model.Quantity))
                       .ReturnsAsync(true);

            var result = await _controller.AddToWishlist(model) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetWishlistItems_ReturnsOkResult()
        {
            var items = new List<WishlistResponseModel>
            {
                new WishlistResponseModel { WishlistId = 1, BookId = 101, Quantity = 2 }
            };

            _wishlistBL.Setup(x => x.GetWishlistItemsAsync(1))
                       .ReturnsAsync(items);

            var result = await _controller.GetWishlistItems() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetWishlistItems_ReturnsNotFound()
        {
            _wishlistBL.Setup(x => x.GetWishlistItemsAsync(1))
                       .ReturnsAsync(new List<WishlistResponseModel>());

            var result = await _controller.GetWishlistItems() as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task RemoveFromWishlist()
        {
            _wishlistBL.Setup(x => x.RemoveFromWishlistAsync(1))
                       .ReturnsAsync(true);

            var result = await _controller.RemoveFromWishlist(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task MoveToCart()
        {
            _wishlistBL.Setup(x => x.MoveToCartAsync(1, 1))
                       .ReturnsAsync(true);

            var result = await _controller.MoveToCart(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }


       
        [Test]
        public async Task GetWishlistItems_ReturnsNotFound_WhenNull()
        {
            _wishlistBL.Setup(x => x.GetWishlistItemsAsync(1))
                       .ReturnsAsync((List<WishlistResponseModel>)null);

            var result = await _controller.GetWishlistItems() as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
