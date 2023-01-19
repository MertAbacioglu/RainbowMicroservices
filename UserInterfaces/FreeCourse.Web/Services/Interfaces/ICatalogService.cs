using FreeCourse.Web.Models.Catalog;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CourseVM>> GetAllCourseAsync();

        Task<List<CategoryVM>> GetAllCategoryAsync();

        Task<List<CourseVM>> GetAllCourseByUserIdAsync(string userId);

        Task<CourseVM> GetByCourseId(string courseId);

        Task<bool> CreateCourseAsync(CourseCreateVM courseCreateVm);

        Task<bool> UpdateCourseAsync(CourseUpdateVM courseUpdateVm);

        Task<bool> DeleteCourseAsync(string courseId);
    }
}
