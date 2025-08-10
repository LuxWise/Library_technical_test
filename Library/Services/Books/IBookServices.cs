using Library.DTO.Books;

namespace Library.Services.Books
{
    public interface IBookServices
    {
        Task<List<BooksListItem>> GetBooks(CancellationToken ct= default);
        Task<BookDetail?> GetBookById(Guid id, CancellationToken ct = default);
        Task<BooksResponse> AddBook(BooksRequest request, CancellationToken ct = default);
        Task<BooksResponse?> UpdateBook(Guid id, BooksRequest request, CancellationToken ct = default);
    }    
}
