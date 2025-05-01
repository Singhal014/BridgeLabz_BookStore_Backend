using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BookStore.Controllers;
using BusinessLayer.Interfaces;
using ModelLayer.Models;

namespace BookStore.Tests.Controllers
{
    public class AdminControllerTests
    {
        private Mock<IUserBL> _mockUserBL;
        private Mock<ILogger<AdminController>> _mockLogger;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserBL = new Mock<IUserBL>();
            _mockLogger = new Mock<ILogger<AdminController>>();
            _controller = new AdminController(_mockUserBL.Object, _mockLogger.Object);
        }

        [Test]
        public async Task RegisterAdmin_ShouldReturnOk_WhenAdminIsRegistered()
        {
            var registerModel = new RegisterModel
            {
                FirstName = "Admin",
                LastName = "Tester",
                Email = "admin@example.com",
                Password = "AdminPass123"
            };

            var userModel = new UserModel
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email
            };

            _mockUserBL.Setup(x => x.RegisterAdminAsync(registerModel)).ReturnsAsync(userModel);

            var result = await _controller.RegisterAdmin(registerModel);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
