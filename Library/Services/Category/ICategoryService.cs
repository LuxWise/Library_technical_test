using Library.DTO.Category;

namespace Library.Services.Category
{
    public interface ICategoryService
    {
        Task<List<CategoryListItem>> GetCategories(CancellationToken ct = default);
        Task<CategoryResponse> addCategory(CategoryRequest request, CancellationToken ct = default);
    }
}