using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IBookRL
    {
        Task<BookEntity> AddBookAsync(BookEntity book);
        Task<IEnumerable<BookEntity>> GetAllBooksAsync();
        Task<BookEntity> GetBookByIdAsync(int id);
        Task<BookEntity> UpdateBookAsync(int id, BookEntity updatedBook);
        Task<bool> DeleteBookAsync(int id);
    }
}