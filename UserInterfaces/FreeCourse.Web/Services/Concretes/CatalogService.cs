using FreeCourse.Shared.Wrappers;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Settings;
using System.Net.Http.Json;

namespace FreeCourse.Web.Services.Concretes
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IPhotoStockService _photoStockService;

        public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService)
        {
            _httpClient = httpClient;
            _photoStockService = photoStockService;
        }

        public async Task<bool> CreateCourseAsync(CourseCreateVM courseCreateVm)
        {
            Models.PhotoStock.PhotoVM resultPhotoService = await _photoStockService.UploadPhoto(courseCreateVm.PhotoFormFile);

            if (resultPhotoService != null)
            {
                courseCreateVm.Picture = resultPhotoService.Url;
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("courses", courseCreateVm);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"courses/{courseId}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryVM>> GetAllCategoryAsync()
        {

            HttpResponseMessage response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Response<List<CategoryVM>>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryVM>>>();

            return responseSuccess.Data;
        }

        public async Task<List<CourseVM>> GetAllCourseAsync()
        {
            //http:localhost:5000/services/catalog/courses
            HttpResponseMessage response = await _httpClient.GetAsync("courses");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Response<List<CourseVM>>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseVM>>>();
            //responseSuccess.Data.ForEach(x =>
            //{
            //    x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            //});
            return responseSuccess.Data;
        }

        public async Task<List<CourseVM>> GetAllCourseByUserIdAsync(string userId)
        {
            //[controller]/GetAllByUserId/{userId}

            
            HttpResponseMessage response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Response<List<CourseVM>>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseVM>>>();

            //responseSuccess.Data.ForEach(x =>
            //{
            //    x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            //});

            return responseSuccess.Data;
        }

        public async Task<CourseVM> GetByCourseId(string courseId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"courses/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Response<CourseVM>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseVM>>();

            //responseSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture);

            return responseSuccess.Data;
        }

        public async Task<bool> UpdateCourseAsync(CourseUpdateVM courseUpdateVm)
        {
            //var resultPhotoService = await _photoStockService.UploadPhoto(courseUpdateVm.PhotoFormFile);

            //if (resultPhotoService != null)
            //{
            //    await _photoStockService.DeletePhoto(courseUpdateVm.Picture);
            //    courseUpdateVm.Picture = resultPhotoService.Url;
            //}

            
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<CourseUpdateVM>("courses", courseUpdateVm);

            return response.IsSuccessStatusCode;
        }
    }
}
