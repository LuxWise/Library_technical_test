using Microsoft.AspNetCore.Mvc;
using Library.Data;

namespace Library.Controllers.Books
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private  readonly LibraryDbContext _context;
        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public List<Book> GetBooks()
        {
            return _context.Book.ToList();
        }
        
        [HttpGet("{id}")]
        public Book GetBook(Guid id)
        {
            if (id == Guid.Empty)
            { 
                throw new ArgumentException("Invalid book ID");
            }
            
            return _context.Book.Find(id) ?? throw new KeyNotFoundException("Book not found");
        }

        [HttpPost]
        public BooksResponse AddBook([FromBody] Book book)
        {
            return new BooksResponse{message = "Book added successfully: " + book.Title};
        }

        [HttpPut("{id}")]
        public BooksResponse UpdateBook(Guid id, [FromBody] Book book)
        {
            return new BooksResponse{message = "Book update successfully: " + book.Title};
        }
        
    }
}