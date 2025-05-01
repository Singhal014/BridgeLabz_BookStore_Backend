using BookStore.Controllers;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using Moq;
using NUnit.Framework;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreTest.ControllerTests
{
    public class AddressControllerTests
    {
        private Mock<IAddressBL> _addressBL;
        private Mock<ILogger<AddressController>> _logger;
        private AddressController _controller;

        [SetUp]
        public void Setup()
        {
            _addressBL = new Mock<IAddressBL>();
            _logger = new Mock<ILogger<AddressController>>();
            _controller = new AddressController(_addressBL.Object, _logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        // ✅ PASSING TEST CASES

        [Test]
        public async Task AddAddressAsync()
        {
            var model = new AddressModel { FirstName = "prankul", LastName = "singhal" };
            var entity = new AddressEntity { FirstName = "prankul", LastName = "singhal", UserId = 1 };

            _addressBL.Setup(x => x.AddAddressAsync(It.IsAny<AddressEntity>())).ReturnsAsync(entity);

            var result = await _controller.AddAddressAsync(model) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetAllAddressesAsync()
        {
            var list = new List<AddressEntity> { new AddressEntity { UserId = 1 } };
            _addressBL.Setup(x => x.GetAllAddressesAsync(1)).ReturnsAsync(list);

            var result = await _controller.GetAllAddressesAsync() as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task UpdateAddressAsync()
        {
            var model = new AddressModel { FirstName = "prankul", LastName = "singhal" };
            var entity = new AddressEntity { FirstName = "prankul", LastName = "singhal", UserId = 1 };

            _addressBL.Setup(x => x.UpdateAddressAsync(1, 1, It.IsAny<AddressEntity>())).ReturnsAsync(entity);

            var result = await _controller.UpdateAddressAsync(1, model) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task DeleteAddressAsync()
        {
            _addressBL.Setup(x => x.DeleteAddressAsync(1, 1)).ReturnsAsync(true);

            var result = await _controller.DeleteAddressAsync(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        // ❌ FAILING TEST CASES

        [Test]
        public async Task AddAddressAsync_Fail()
        {
            var model = new AddressModel { FirstName = "fail", LastName = "case" };

            _addressBL.Setup(x => x.AddAddressAsync(It.IsAny<AddressEntity>())).ReturnsAsync((AddressEntity)null);

            var result = await _controller.AddAddressAsync(model) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task GetAllAddressesAsync_Fail()
        {
            _addressBL.Setup(x => x.GetAllAddressesAsync(1)).ReturnsAsync((List<AddressEntity>)null);

            var result = await _controller.GetAllAddressesAsync() as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
