using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Wrappers;
using StackExchange.Redis;
using System.Text.Json;

namespace FreeCourse.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService;
        public BasketService(RedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Response<bool>> DeleteBasket(string userId)
        {
            bool status = await _redisService.GetDb().KeyDeleteAsync(userId);
            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket not found", 404);
        }

        public async Task<Response<BasketDto>> GetBasket(string userId)
        {
            RedisValue existedBasked = await _redisService.GetDb().StringGetAsync(userId);
            if (existedBasked.IsNullOrEmpty)
                return Response<BasketDto>.Fail("Basket not found", 404);
            return Response<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(existedBasked), 200);
        }

        public async Task<Response<bool>> SaveOrUpdate(BasketDto basketDto)
        {
            bool status = await _redisService.GetDb().StringSetAsync(basketDto.UserId, JsonSerializer.Serialize(basketDto));
            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket could not be updated or saved", 500);
        }
    }
}
