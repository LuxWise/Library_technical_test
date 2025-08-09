using Microsoft.AspNetCore.Mvc;
using Library.Data;

namespace Library.Controllers.Categories
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public CategoriesController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Category> GetCategories()
        {
            return _context.Category.ToList();
        }
        
    }
    
}