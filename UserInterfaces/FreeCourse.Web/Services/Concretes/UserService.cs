using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserVM> GetUser()
        {
            return await _httpClient.GetFromJsonAsync<UserVM>("/api/users/getuser");
        }
    }
}
