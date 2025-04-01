using System.Collections.Generic;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using ModelLayer.Models;
using System;
using System.Linq;
using BusinessLayer.Interfaces;
using RepoLayer.Interface;

namespace BusinessLayer.Services
{
    public class BookBL : IBookBL
    {
        private readonly IBookRL _bookRL;
        private readonly IUserRL _userRL;

        public BookBL(IBookRL bookRL, IUserRL userRL)
        {
            _bookRL = bookRL;
            _userRL = userRL;
        }

        public BookModel AddBook(BookRequestModel model, int userId)
        {
            if (_userRL.GetUserById(userId) == null)
                throw new KeyNotFoundException("User not found");

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

            var createdBook = _bookRL.AddBook(bookEntity);
            return MapToModel(createdBook);
        }

        public IEnumerable<BookModel> GetAllBooks()
        {
            return _bookRL.GetAllBooks()
                .Select(book => MapToModel(book))
                .ToList();
        }

        public BookModel GetBookById(int id)
        {
            var bookEntity = _bookRL.GetBookById(id);
            return bookEntity == null ? null : MapToModel(bookEntity);
        }

        public BookModel UpdateBook(int id, BookRequestModel model, int userId)
        {
            var existingBook = _bookRL.GetBookById(id);
            if (existingBook == null || existingBook.UserId != userId)
                return null;

            existingBook.Title = model.Title;
            existingBook.AuthorName = model.AuthorName;
            existingBook.Description = model.Description;
            existingBook.Price = (int)model.Price;
            existingBook.Quantity = model.Quantity;
            existingBook.Image = model.Image ?? existingBook.Image;

            var updatedBook = _bookRL.UpdateBook(id, existingBook);
            return MapToModel(updatedBook);
        }

        public bool DeleteBook(int id, int userId)
        {
            var book = _bookRL.GetBookById(id);
            if (book == null || book.UserId != userId)
                return false;

            return _bookRL.DeleteBook(id);
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
}