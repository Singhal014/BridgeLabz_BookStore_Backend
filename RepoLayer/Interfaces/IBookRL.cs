using RepoLayer.Entity;

namespace RepoLayer.Interface
{
    public interface IBookRL
    {
        BookEntity AddBook(BookEntity book);
        IEnumerable<BookEntity> GetAllBooks();
        BookEntity GetBookById(int id);
        BookEntity UpdateBook(int id, BookEntity updatedBook);
        bool DeleteBook(int id);
    }
}