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
    public class BookControllerTests
    {
        private Mock<IBookBL> bookBL;
        private Mock<ILogger<BookController>> logger;
        private BookController controller;

        [SetUp]
        public void Setup()
        {
            bookBL = new Mock<IBookBL>();
            logger = new Mock<ILogger<BookController>>();
            controller = new BookController(bookBL.Object, logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Test]
        public async Task AddBook_ReturnsOk()
        {
            var book = new BookRequestModel
            {
                Title = "Test Book",
                AuthorName = "Author Test",
                Price = 199.99m,
                Quantity = 5,
                Description = "This is a sample book for testing.",
                Image = "image.jpg"
            };

            var addedBook = new BookModel
            {
                Id = 1,
                Title = "Test Book",
                Author = "Author Test",
                Price = 199,
                Quantity = 5,
                Description = "This is a sample book for testing.",
                Image = "image.jpg"
            };

            bookBL.Setup(x => x.AddBookAsync(book, 1)).ReturnsAsync(addedBook);

            var result = await controller.AddBook(book) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var data = result.Value as ResponseModel<BookModel>;
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Book added successfully", data.Message);
        }

        [Test]
        public async Task GetAllBooks_ReturnsList()
        {
            var books = new List<BookModel> { new BookModel { Title = "Test" } };
            bookBL.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(books);

            var result = await controller.GetAllBooks() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var data = result.Value as ResponseModel<IEnumerable<BookModel>>;
            Assert.IsTrue(data.Success);
        }

        [Test]
        public async Task GetBookById_ReturnsBook()
        {
            var book = new BookModel { Id = 1, Title = "Test" };
            bookBL.Setup(x => x.GetBookByIdAsync(1)).ReturnsAsync(book);

            var result = await controller.GetBookById(1) as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as ResponseModel<BookModel>;
            Assert.IsTrue(data.Success);
            Assert.AreEqual("Book retrieved successfully", data.Message);
        }

        [Test]
        public async Task UpdateBook_ReturnsOk()
        {
            var book = new BookRequestModel { Title = "Updated" };
            var updatedBook = new BookModel { Id = 1, Title = "Updated" };

            bookBL.Setup(x => x.UpdateBookAsync(1, book, 1)).ReturnsAsync(updatedBook);

            var result = await controller.UpdateBook(1, book) as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as ResponseModel<BookModel>;
            Assert.IsTrue(data.Success);
        }

        [Test]
        public async Task DeleteBook_ReturnsOk()
        {
            bookBL.Setup(x => x.DeleteBookAsync(1, 1)).ReturnsAsync(true);

            var result = await controller.DeleteBook(1) as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as ResponseModel<string>;
            Assert.IsTrue(data.Success);
        }

        

        

        [Test]
        public async Task GetBookById_ReturnsNotFound()
        {
            bookBL.Setup(x => x.GetBookByIdAsync(99)).ReturnsAsync((BookModel)null);

            var result = await controller.GetBookById(99) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task UpdateBook_ReturnsNotFound()
        {
            var book = new BookRequestModel { Title = "Nonexistent" };

            bookBL.Setup(x => x.UpdateBookAsync(99, book, 1)).ReturnsAsync((BookModel)null);

            var result = await controller.UpdateBook(99, book) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task DeleteBook_ReturnsNotFound()
        {
            bookBL.Setup(x => x.DeleteBookAsync(99, 1)).ReturnsAsync(false);

            var result = await controller.DeleteBook(99) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
