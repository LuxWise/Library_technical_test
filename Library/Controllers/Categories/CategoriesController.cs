using Library.DTO.Category;
using Microsoft.AspNetCore.Mvc;
using Library.Services.Category;

namespace Library.Controllers.Categories
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _category;

        public CategoriesController(ICategoryService category)
        {
            _category = category;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> GetCategories(CancellationToken ct)
        {
            var resp = await _category.GetCategories(ct);
            return Ok(resp);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> AddCategory([FromBody] CategoryRequest request,
            CancellationToken ct)
        {
            var resp = await _category.addCategory(request, ct);
            return Ok(resp);
        }
    }
    
}