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
        public String GetBook(int id)
        {
            return id.ToString();
        }
    }
}