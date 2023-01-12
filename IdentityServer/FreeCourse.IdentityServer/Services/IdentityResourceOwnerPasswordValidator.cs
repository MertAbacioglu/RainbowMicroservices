using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(context.UserName);
            if (user == null)
            {
                Dictionary<string, object> errors = new Dictionary<string, object>
                {
                    { "errors", new List<string> { "Username or password is wrong" } }
                };
                context.Result.CustomResponse = errors;
                return;
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, context.Password);
            
            if (!isPasswordValid)
            {
                Dictionary<string, object> errors = new Dictionary<string, object>
                {
                    { "errors", new List<string> { "Username or password is wrong" } }
                };
                context.Result.CustomResponse = errors;
                return;
            }

            context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);

        }
    }
}
