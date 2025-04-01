using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IBookBL _bookBL;

        public BookController(IBookBL bookBL)
        {
            _bookBL = bookBL;
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddBook([FromBody] BookRequestModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid user token");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID in token");
                }

                var result = _bookBL.AddBook(model, userId);
                return Ok(new { message = "Book added successfully", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetAllBooks()
        {
            var result = _bookBL.GetAllBooks();
            return Ok(result);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetBookById(int id)
        {
            var result = _bookBL.GetBookById(id);
            if (result == null)
            {
                return NotFound(new { message = "Book not found" });
            }
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateBook(int id, [FromBody] BookRequestModel updatedBook)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("Id").Value);
                var result = _bookBL.UpdateBook(id, updatedBook, userId);

                if (result == null)
                {
                    return NotFound(new { message = "Book not found or not owned by user" });
                }

                return Ok(new { message = "Book updated successfully", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("Id").Value);
                var result = _bookBL.DeleteBook(id, userId);

                if (!result)
                {
                    return NotFound(new { message = "Book not found or not owned by user" });
                }

                return Ok(new { message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
