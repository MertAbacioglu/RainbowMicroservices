using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers
{

    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //di for user manager
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            ApplicationUser user = new ApplicationUser
            {
                City = signUpDto.City,
                Email = signUpDto.Email,
                UserName = signUpDto.UserName
            };
            IdentityResult result = _userManager.CreateAsync(user, signUpDto.Password).Result;
            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContentDto>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }

            return NoContent();

        }

        [HttpGet]
        public async Task<IActionResult> GetUser()//delege araya girip token'i  header'a ekleyecek
        {
            Claim userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null) return BadRequest();

            ApplicationUser user = await _userManager.FindByIdAsync(userIdClaim.Value);

            if (user == null) return BadRequest();

            return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });
        }
    }
}
