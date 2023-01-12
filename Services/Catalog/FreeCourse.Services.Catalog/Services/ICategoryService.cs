using FreeCourse.Services.Catalog.Dtos.Category;
using FreeCourse.Shared.Wrappers;

namespace FreeCourse.Services.Catalog.Services
{
    public interface ICategoryService
    {
        Task<Response<List<CategoryDto>>> GetAllAsync();

        Task<Response<CategoryDto>> CreateAsync(CategoryCreateDto category);

        Task<Response<CategoryDto>> GetByIdAsync(string id);
    }
}
