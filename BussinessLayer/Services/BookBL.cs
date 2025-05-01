using BusinessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;

public class BookBL : IBookBL
{
    private readonly IBookRL _bookRL;
    private readonly IUserRL _userRL;
    private readonly ILogger<BookBL> _logger;

    public BookBL(IBookRL bookRL, IUserRL userRL, ILogger<BookBL> logger)
    {
        _bookRL = bookRL;
        _userRL = userRL;
        _logger = logger;
    }

    public async Task<BookModel> AddBookAsync(BookRequestModel model, int userId)
    {
        try
        {
            _logger.LogInformation("Adding a new book for userId: {UserId}", userId);
            var user = await _userRL.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with userId: {UserId} not found", userId);
                throw new KeyNotFoundException("User not found");
            }

            var bookEntity = new BookEntity
            {
                Title = model.Title,
                AuthorName = model.AuthorName,
                Price = (int)model.Price,
                Quantity = model.Quantity,
                Description = model.Description,
                Image = model.Image,
                UserId = userId
            };

            var createdBook = await _bookRL.AddBookAsync(bookEntity);
            _logger.LogInformation("Book {BookTitle} added successfully by userId: {UserId}", createdBook.Title, userId);
            return MapToModel(createdBook);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding book for userId: {UserId}", userId);
            throw new Exception("An error occurred while adding the book.", ex);
        }
    }

    public async Task<IEnumerable<BookModel>> GetAllBooksAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all books.");
            var books = await _bookRL.GetAllBooksAsync();
            _logger.LogInformation("Total {BookCount} books retrieved.", books.Count());
            return books.Select(book => MapToModel(book)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all books.");
            throw new Exception("An error occurred while fetching all books.", ex);
        }
    }

    public async Task<BookModel> GetBookByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching book with id: {BookId}", id);
            var bookEntity = await _bookRL.GetBookByIdAsync(id);

            if (bookEntity == null)
            {
                _logger.LogWarning("Book with id: {BookId} not found", id);
                return null;
            }

            return MapToModel(bookEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching book with id: {BookId}", id);
            throw new Exception("An error occurred while fetching the book.", ex);
        }
    }

    public async Task<BookModel> UpdateBookAsync(int id, BookRequestModel model, int userId)
    {
        try
        {
            _logger.LogInformation("Updating book with id: {BookId} for userId: {UserId}", id, userId);
            var existingBook = await _bookRL.GetBookByIdAsync(id);

            if (existingBook == null || existingBook.UserId != userId)
            {
                _logger.LogWarning("Book update failed. Book not found or userId mismatch. BookId: {BookId}, UserId: {UserId}", id, userId);
                return null;
            }

            existingBook.Title = model.Title;
            existingBook.AuthorName = model.AuthorName;
            existingBook.Description = model.Description;
            existingBook.Price = (int)model.Price;
            existingBook.Quantity = model.Quantity;
            existingBook.Image = model.Image ?? existingBook.Image;

            var updatedBook = await _bookRL.UpdateBookAsync(id, existingBook);
            _logger.LogInformation("Book {BookId} updated successfully by userId: {UserId}", id, userId);

            return MapToModel(updatedBook);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book with id: {BookId} by userId: {UserId}", id, userId);
            throw new Exception("An error occurred while updating the book.", ex);
        }
    }

    public async Task<bool> DeleteBookAsync(int id, int userId)
    {
        try
        {
            _logger.LogInformation("Deleting book with id: {BookId} for userId: {UserId}", id, userId);
            var book = await _bookRL.GetBookByIdAsync(id);

            if (book == null || book.UserId != userId)
            {
                _logger.LogWarning("Book deletion failed. Book not found or userId mismatch. BookId: {BookId}, UserId: {UserId}", id, userId);
                return false;
            }

            bool result = await _bookRL.DeleteBookAsync(id);
            _logger.LogInformation("Book {BookId} deleted successfully by userId: {UserId}", id, userId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting book with id: {BookId} by userId: {UserId}", id, userId);
            throw new Exception("An error occurred while deleting the book.", ex);
        }
    }

    private BookModel MapToModel(BookEntity book)
    {
        return new BookModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.AuthorName,
            Description = book.Description,
            Price = book.Price,
            Quantity = book.Quantity,
            Image = book.Image
        };
    }
}
