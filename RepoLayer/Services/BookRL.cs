using Microsoft.EntityFrameworkCore;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interface;
using RepoLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RepoLayer.Services
{
    public class BookRL : IBookRL
    {
        private readonly ApplicationDbContext _context;

        public BookRL(ApplicationDbContext context)
        {
            _context = context;
        }

        public BookEntity AddBook(BookEntity book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return book;
        }

        public IEnumerable<BookEntity> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public BookEntity GetBookById(int id)
        {
            return _context.Books.FirstOrDefault(b => b.Id == id);
        }

        public BookEntity UpdateBook(int id, BookEntity updatedBook)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                book.Title = updatedBook.Title;
                book.Description = updatedBook.Description;
                book.AuthorName = updatedBook.AuthorName;
                book.Quantity = updatedBook.Quantity;
                book.Price = updatedBook.Price;
                book.Image = updatedBook.Image;
                _context.SaveChanges();
            }
            return book;
        }

        public bool DeleteBook(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
