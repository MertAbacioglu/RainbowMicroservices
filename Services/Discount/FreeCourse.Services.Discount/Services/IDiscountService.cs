using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Wrappers;

namespace FreeCourse.Services.Discount.Services
{
    public interface IDiscountService
    {
        Task<Response<List<Models.Discount>>> GetAll();

        Task<Response<Models.Discount>> GetById(int id);

        Task<Response<NoContentDto>> Save(Models.Discount discount);

        Task<Response<NoContentDto>> Update(Models.Discount discount);

        Task<Response<NoContentDto>> Delete(int id);

        Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId);
    }
}
