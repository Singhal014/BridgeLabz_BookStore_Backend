using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BookStore.Controllers;
using BusinessLayer.Interfaces;
using ModelLayer.Models;

namespace BookStoreTests
{
    public class UserControllerTests
    {
        private Mock<IUserBL> _mockUserBL;
        private Mock<ILogger<UserController>> _mockLogger;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserBL = new Mock<IUserBL>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserBL.Object, _mockLogger.Object);
        }

        [Test]
        public async Task TestRegisterOk()
        {
            var model = new RegisterModel { FirstName = "prankul", LastName = "singhal", Email = "prankulsinghal014@gmail.com", Password = "123456" };
            var user = new UserModel { FirstName = "prankul", LastName = "singhal", Email = "prankulsinghal014@gmail.com" };

            _mockUserBL.Setup(x => x.RegisterUserAsync(model)).ReturnsAsync(user);

            var result = await _controller.Register(model);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        

        [Test]
        public async Task TestRegisterFail()
        {
            var model = new RegisterModel { FirstName = "prankul", LastName = "singhal", Email = "p@s.com", Password = "abcdef" };

            _mockUserBL.Setup(x => x.RegisterUserAsync(model)).ReturnsAsync((UserModel)null);

            var result = await _controller.Register(model);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TestLoginOk()
        {
            var model = new LoginModel { Email = "prankul", Password = "Password123" };
            var user = new UserModel { FirstName = "prankul", LastName = "prankul", Email = model.Email };

            _mockUserBL.Setup(x => x.LoginAsync(model)).ReturnsAsync(user);

            var result = await _controller.Login(model);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }



        [Test]
        public async Task TestForgotOk()
        {
            var email = "forgot@singhal.com";
            var model = new ForgotPasswordModel { Email = email };
            var token = "mock-token";

            _mockUserBL.Setup(x => x.ForgotPasswordAsync(email)).ReturnsAsync(token);

            var result = await _controller.ForgotPassword(model);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }


        [Test]
        public async Task TestResetOk()
        {
            var model = new ResetPasswordModel { Email = "reset@singhal.com", Otp = "123456", NewPassword = "NewPass123" };

            _mockUserBL.Setup(x => x.ResetPasswordAsync(model)).ReturnsAsync(true);

            var result = await _controller.ResetPassword(model);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TestResetFail()
        {
            var model = new ResetPasswordModel { Email = "reset@singhal.com", Otp = "wrong", NewPassword = "weak" };

            _mockUserBL.Setup(x => x.ResetPasswordAsync(model)).ReturnsAsync(false);

            var result = await _controller.ResetPassword(model);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
