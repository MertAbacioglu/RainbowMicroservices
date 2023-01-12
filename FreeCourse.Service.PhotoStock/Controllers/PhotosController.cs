using FreeCourse.Service.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Service.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        //create async  action takes parameter as  IFormFile
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile file,CancellationToken cancellationToken)
        {
            //check file is null or not
            if (file == null)
            {
                return CreateActionResultInstance(Response<PhotoDto>.Fail("File is null", 400));
            }

            //check file is image or not
            if (!file.ContentType.Contains("image"))
            {
                return CreateActionResultInstance(Response<PhotoDto>.Fail("File is not image", 400));
            }

            //create path for save image
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", file.FileName);

            //create stream for save image
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream,cancellationToken);
            }

            string returnPath = "photos/" + file.FileName;
            PhotoDto photoDto = new() { Url = returnPath };
            return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
        }

        //create async action for delete photo
        [HttpDelete]
        public async Task<IActionResult> PhotoDelete(string photoUrl)
        {
            //check photoUrl is null or not
            if (string.IsNullOrEmpty(photoUrl))
            {
                return CreateActionResultInstance(Response<NoContentDto>.Fail("PhotoUrl is null", 400));
            }

            //create path for delete image
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            //check file is exist or not
            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(Response<NoContentDto>.Fail("File is not exist", 400));
            }
            System.IO.File.Delete(path);
            return CreateActionResultInstance(Response<NoContentDto>.Success(204));
        }
    }
}
