using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Wrappers;

namespace FreeCourse.Services.Basket.Services
{
    public interface IBasketService
    {
        public Task<Response<bool>> DeleteBasket(string userId);
        public Task<Response<BasketDto>> GetBasket(string userId);
        public Task<Response<bool>> SaveOrUpdate(BasketDto basketDto);

    }
}
