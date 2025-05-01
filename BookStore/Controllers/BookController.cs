    using BusinessLayer.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using ModelLayer.Models;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace BookStore.Controllers
    {
        [Route("[controller]")]
        [ApiController]
        [Authorize]
        public class BookController : ControllerBase
        {
            private readonly IBookBL _bookBL;
            private readonly ILogger<BookController> _logger;

            public BookController(IBookBL bookBL, ILogger<BookController> logger)
            {
                _bookBL = bookBL;
                _logger = logger;
            }

            /// <summary>
            /// Adds a new book to the system (Admin only)
            /// </summary>
            [HttpPost]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> AddBook([FromBody] BookRequestModel model)
            {
                try
                {
                    var userIdClaim = User.FindFirst("Id")?.Value;
                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        _logger.LogWarning("Invalid user token.");
                        return Unauthorized(new ResponseModel<string>
                        {
                            Success = false,
                            Message = "Invalid user token",
                            Data = null
                        });
                    }

                    if (!int.TryParse(userIdClaim, out int userId))
                    {
                        _logger.LogWarning("Invalid user ID in token.");
                        return BadRequest(new ResponseModel<string>
                        {
                            Success = false,
                            Message = "Invalid user ID in token",
                            Data = null
                        });
                    }

                    var result = await _bookBL.AddBookAsync(model, userId);
                    _logger.LogInformation("Book added successfully by user {UserId}.", userId);

                    return Ok(new ResponseModel<BookModel>
                    {
                        Success = true,
                        Message = "Book added successfully",
                        Data = result
                    });
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogError(ex, "Book addition failed: {Message}", ex.Message);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Internal server error while adding book.");
                    return StatusCode(500, new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Internal server error",
                        Data = null
                    });
                }
            }

            /// <summary>
            /// Retrieves all available books in the system
            /// </summary>
            [HttpGet]
            [Authorize(Roles = "Admin,User")]
            public async Task<IActionResult> GetAllBooks()
            {
                try
                {
                    var result = await _bookBL.GetAllBooksAsync();
                    _logger.LogInformation("All books retrieved successfully.");
                    return Ok(new ResponseModel<IEnumerable<BookModel>>
                    {
                        Success = true,
                        Message = "Books retrieved successfully",
                        Data = result
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Internal server error while retrieving books.");
                    return StatusCode(500, new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Internal server error",
                        Data = null
                    });
                }
            }

            /// <summary>
            /// Gets details of a specific book by its ID
            /// </summary>
            [HttpGet("{id}")]
            [Authorize(Roles = "Admin,User")]
            public async Task<IActionResult> GetBookById(int id)
            {
                try
                {
                    var result = await _bookBL.GetBookByIdAsync(id);
                    if (result == null)
                    {
                        _logger.LogWarning("Book with ID {BookId} not found.", id);
                        return NotFound(new ResponseModel<string>
                        {
                            Success = false,
                            Message = "Book not found",
                            Data = null
                        });
                    }

                    _logger.LogInformation("Book with ID {BookId} retrieved successfully.", id);
                    return Ok(new ResponseModel<BookModel>
                    {
                        Success = true,
                        Message = "Book retrieved successfully",
                        Data = result
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Internal server error while retrieving book with ID {BookId}.", id);
                    return StatusCode(500, new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Internal server error",
                        Data = null
                    });
                }
            }

            /// <summary>
            /// Updates an existing book's information (Admin only)
            /// </summary>
            [HttpPut("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> UpdateBook(int id, [FromBody] BookRequestModel updatedBook)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst("Id").Value);
                    var result = await _bookBL.UpdateBookAsync(id, updatedBook, userId);

                    if (result == null)
                    {
                        _logger.LogWarning("Book with ID {BookId} not found or not owned by user {UserId}.", id, userId);
                        return NotFound(new ResponseModel<string>
                        {
                            Success = false,
                            Message = "Book not found or not owned by user",
                            Data = null
                        });
                    }

                    _logger.LogInformation("Book with ID {BookId} updated successfully by user {UserId}.", id, userId);
                    return Ok(new ResponseModel<BookModel>
                    {
                        Success = true,
                        Message = "Book updated successfully",
                        Data = result
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Internal server error while updating book with ID {BookId}.", id);
                    return StatusCode(500, new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Internal server error",
                        Data = null
                    });
                }
            }

            /// <summary>
            /// Deletes a book from the system (Admin only)
            /// </summary>
            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> DeleteBook(int id)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst("Id").Value);
                    var result = await _bookBL.DeleteBookAsync(id, userId);

                    if (!result)
                    {
                        _logger.LogWarning("Book with ID {BookId} not found or not owned by user {UserId}.", id, userId);
                        return NotFound(new ResponseModel<string>
                        {
                            Success = false,
                            Message = "Book not found or not owned by user",
                            Data = null
                        });
                    }

                    _logger.LogInformation("Book with ID {BookId} deleted successfully by user {UserId}.", id, userId);
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Book deleted successfully",
                        Data = null
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Internal server error while deleting book with ID {BookId}.", id);
                    return StatusCode(500, new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Internal server error",
                        Data = null
                    });
                }
            }
        }
    }