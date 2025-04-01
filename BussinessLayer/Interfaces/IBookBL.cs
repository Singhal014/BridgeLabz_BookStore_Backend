using ModelLayer.Models;
using RepoLayer.Entity;
using System.Collections.Generic;

namespace BusinessLayer.Interfaces
{
    public interface IBookBL
    {
        BookModel AddBook(BookRequestModel bookModel, int userId);
        IEnumerable<BookModel> GetAllBooks();
        BookModel GetBookById(int id);
        BookModel UpdateBook(int id, BookRequestModel updatedBookModel, int userId);
        bool DeleteBook(int id, int userId);
    }
}
