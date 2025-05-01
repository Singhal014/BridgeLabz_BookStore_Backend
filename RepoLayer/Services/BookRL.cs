using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class BookRL : IBookRL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookRL> _logger;

        public BookRL(ApplicationDbContext context, ILogger<BookRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BookEntity> AddBookAsync(BookEntity book)
        {
            try
            {
                _logger.LogInformation("Adding a new book: {Title} by {Author}", book.Title, book.AuthorName);
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book added successfully with ID {BookId}", book.Id);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new book.");
                throw;
            }
        }

        public async Task<IEnumerable<BookEntity>> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all books from the database.");
                return await _context.Books.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all books.");
                throw;
            }
        }

        public async Task<BookEntity> GetBookByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching book with ID {BookId}", id);
                return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book by ID {BookId}", id);
                throw;
            }
        }

        public async Task<BookEntity> UpdateBookAsync(int id, BookEntity updatedBook)
        {
            try
            {
                _logger.LogInformation("Updating book with ID {BookId}", id);
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
                if (book != null)
                {
                    book.Title = updatedBook.Title;
                    book.Description = updatedBook.Description;
                    book.AuthorName = updatedBook.AuthorName;
                    book.Quantity = updatedBook.Quantity;
                    book.Price = updatedBook.Price;
                    book.Image = updatedBook.Image;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Book with ID {BookId} updated successfully", id);
                }
                else
                {
                    _logger.LogWarning("Book with ID {BookId} not found", id);
                }
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID {BookId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting book with ID {BookId}", id);
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Book with ID {BookId} deleted successfully", id);
                    return true;
                }
                _logger.LogWarning("Book with ID {BookId} not found", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID {BookId}", id);
                throw;
            }
        }
    }
}
