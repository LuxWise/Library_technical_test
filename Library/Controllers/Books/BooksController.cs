using Microsoft.AspNetCore.Mvc;
using Library.DTO.Books;
using Library.Services.Books;
using Microsoft.AspNetCore.Authorization;

namespace Library.Controllers.Books
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private  readonly IBookServices _book;
        public BooksController(IBookServices book)
        {
            _book = book;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<BooksListItem>>> GetBooks()
        {
            var resp = await _book.GetBooks();
            return Ok(resp);        
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDetail?>> GetBook(Guid id)
        {
            var resp = await _book.GetBookById(id);
            if (resp is null) return NotFound(new { message = "Book not found" });
            return Ok(resp);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BooksResponse>> AddBook([FromBody] BooksRequest book)
        {
            var resp = await _book.AddBook(book);
            return Ok(resp);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<BooksResponse?>> UpdateBook(Guid id, [FromBody] BooksRequest book)
        {
            var resp = await _book.UpdateBook(id, book);
            if (resp is null) return NotFound(new { message = "Book doesn't update" });
            return Ok(resp);        
        }
        
    }
}