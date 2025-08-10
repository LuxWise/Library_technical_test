using Library.DTO.Category;
using Microsoft.AspNetCore.Mvc;
using Library.Services.Category;
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResponse>>> GetCategories(CancellationToken ct)
        {
            var resp = await _category.GetCategories(ct);
            return Ok(resp);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryResponse>> AddCategory([FromBody] CategoryRequest request,
            CancellationToken ct)
        {
            var resp = await _category.addCategory(request, ct);
            return Ok(resp);
        }
    }
    
}