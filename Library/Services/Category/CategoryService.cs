using Library.Data;
using Library.DTO.Category;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly LibraryDbContext _db;
        public CategoryService(LibraryDbContext db) => _db = db;

        public async Task<List<CategoryListItem>> GetCategories(CancellationToken ct = default)
        {
            return await _db.Category
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryListItem(
                    c.Id,
                    c.Name
                ))
                .ToListAsync(ct);

            
        }

        public async Task<CategoryResponse> addCategory(CategoryRequest request, CancellationToken ct = default)
        {
            var categoryExists = await _db.Category
                .AsNoTracking()
                .AnyAsync(c => c.Name == request.CategoryName, ct);
            
            if (categoryExists)
                throw new InvalidOperationException("Category already exists.");

            var category = new Model.Category()
            {
                Name = request.CategoryName
            };
            
            _db.Category.Add(category);
            await _db.SaveChangesAsync(ct);
            
            return new CategoryResponse()
            {
                Message = "Category added successfully.",
                CategoryName = category.Name
            };
        }
        
        
    }
}