using ModelLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IBookBL
    {
        Task<BookModel> AddBookAsync(BookRequestModel bookModel, int userId);
        Task<IEnumerable<BookModel>> GetAllBooksAsync();
        Task<BookModel> GetBookByIdAsync(int id);
        Task<BookModel> UpdateBookAsync(int id, BookRequestModel updatedBookModel, int userId);
        Task<bool> DeleteBookAsync(int id, int userId);
    }
}