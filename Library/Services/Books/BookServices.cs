using Library.Data;
using Library.DTO.Books;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Books
{ 
    public class BookServices : IBookServices
    {
        private readonly LibraryDbContext _db;
        public BookServices(LibraryDbContext db)
        {
            _db = db;
        }
        
        public async Task<List<BooksListItem>> GetBooks( CancellationToken ct = default)
        {
            return await _db.Book
                .AsNoTracking()
                .OrderBy(b => b.Category)
                .Select(b => new BooksListItem(
                    b.Id,
                    b.Title,
                    b.Author,
                    b.ISBN,
                    b.PublicationYear,
                    b.Category.Name,
                    b.Available
                    ))
                .ToListAsync(ct);
        }
        
        public async Task<BookDetail?> GetBookById(Guid id, CancellationToken ct = default)
        {
            return await _db.Book
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(b => b.Id == id)
                .Select(b => new BookDetail(
                    b.Id,
                    b.Title,
                    b.Author,
                    b.ISBN,
                    b.PublicationYear,
                    b.CategoryId,
                    b.Category.Name,
                    b.Available,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync();
        }
        
        public async Task<BooksResponse> AddBook(BooksRequest request, CancellationToken ct = default)
        {
            var categoryExists = await _db.Category
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId, ct);

            if (!categoryExists)
                throw new InvalidOperationException("Category does not exist.");
            
            
            var isbnExists = await _db.Book
                .AsNoTracking()
                .AnyAsync(b => b.ISBN == request.ISBN, ct);
            if (isbnExists)
                throw new InvalidOperationException("ISBN already exists.");
            
            var book = new Book
            {
                Title = request.Title.Trim(),
                Author = request.Author.Trim(),
                ISBN = request.ISBN.Trim(),
                PublicationYear = request.PublicationYear,
                CategoryId = request.CategoryId,
                Available = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Book.Add(book);
            await _db.SaveChangesAsync(ct);

            var categoryName = await _db.Category
                .AsNoTracking()
                .Where(c => c.Id == request.CategoryId)
                .Select(c => c.Name)
                .FirstAsync(ct);

            return new BooksResponse
            {
                message = "Book added successfully",
                Book = new BookDetail(
                    book.Id,
                    book.Title,
                    book.Author,
                    book.ISBN,
                    book.PublicationYear,
                    book.CategoryId,
                    categoryName,
                    book.Available,
                    book.CreatedAt,
                    book.UpdatedAt
                )
            };
        }

        public async Task<BooksResponse?> UpdateBook(Guid id, BooksRequest request, CancellationToken ct = default)
        {
            var book = await _db.Book.FirstOrDefaultAsync(b => b.Id == id, ct);
            if (book is null) return null;

            var categoryExists = await _db.Category
                .AsNoTracking()
                .AnyAsync(c => c.Id == request.CategoryId, ct);

            if (!categoryExists)
                throw new InvalidOperationException("Category does not exist.");

            if (!string.Equals(book.ISBN, request.ISBN, StringComparison.OrdinalIgnoreCase))
            {
                var isbnExists = await _db.Book
                    .AsNoTracking()
                    .AnyAsync(b => b.ISBN == request.ISBN && b.Id != id, ct);
                if (isbnExists)
                    throw new InvalidOperationException("ISBN already exists.");
            }
            
            book.Title = request.Title.Trim();
            book.Author = request.Author.Trim();
            book.ISBN = request.ISBN.Trim();
            book.PublicationYear = request.PublicationYear;
            book.CategoryId = request.CategoryId;
            book.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            var categoryName = await _db.Category
                .AsNoTracking()
                .Where(c => c.Id == request.CategoryId)
                .Select(c => c.Name)
                .FirstAsync(ct);
            
            return new BooksResponse
            {
                message = "Book updated successfully",
                Book = new BookDetail(
                    book.Id,
                    book.Title,
                    book.Author,
                    book.ISBN,
                    book.PublicationYear,
                    book.CategoryId,
                    categoryName,
                    book.Available,
                    book.CreatedAt,
                    book.UpdatedAt
                )
            };

        }


    }    
}
