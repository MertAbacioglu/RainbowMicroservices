using FreeCourse.Shared.Wrappers;
using FreeCourse.Web.Models.PhotoStock;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Services.Concretes
{
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;

        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        
        public async Task<PhotoVM> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length <= 0)
            {
                return null;
            }
            //203802340234.jpg
            string randonFilename = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";

            using MemoryStream ms = new MemoryStream();

            await photo.CopyToAsync(ms);

            MultipartFormDataContent multipartContent = new()
            {
                { new ByteArrayContent(ms.ToArray()), "photo", randonFilename }
            };

            HttpResponseMessage response = await _httpClient.PostAsync("photos", multipartContent);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Response<PhotoVM>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoVM>>();

            return responseSuccess.Data;
        }


        public async Task<bool> DeletePhoto(string photoUrl)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");
            return response.IsSuccessStatusCode;
        }
    }
}
