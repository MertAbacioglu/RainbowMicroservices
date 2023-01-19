using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Wrappers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Settings;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace FreeCourse.Web.Services.Concretes
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientConfiguration _clientConfiguration;
        private readonly ServiceApiConfiguration _serviceAPIConfiguration;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientConfiguration> clientConfiguration, IOptions<ServiceApiConfiguration> serviceAPIConfiguration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _clientConfiguration = clientConfiguration.Value;
            _serviceAPIConfiguration = serviceAPIConfiguration.Value;//ToDo : add  this field as singleton at startup side
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            DiscoveryDocumentResponse discoDocResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceAPIConfiguration.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }

            });
            if (discoDocResponse.IsError)
            {
                throw discoDocResponse.Exception;
            }

            TokenResponse tokenResponse = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoDocResponse.TokenEndpoint,
                ClientId = _clientConfiguration.WebClientForUsers.ClientId,
                ClientSecret = _clientConfiguration.WebClientForUsers.ClientSecret,
                RefreshToken = _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token").Result
            });

            if (tokenResponse.IsError)
            {
                return null;
            }

            List<AuthenticationToken> authTokens = new()
            {
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.AccessToken, Value = tokenResponse.AccessToken },
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.RefreshToken, Value = tokenResponse.RefreshToken },
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToString("o",CultureInfo.InvariantCulture) }
            };

            AuthenticateResult authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            authenticateResult.Properties.StoreTokens(authTokens);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal, authenticateResult.Properties);

            return tokenResponse;//ihtiyac olabilir
        }

        public async Task RevokeRefreshToken()
        {
            DiscoveryDocumentResponse discoDocResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceAPIConfiguration.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }

            });
            if (discoDocResponse.IsError)
            {
                throw discoDocResponse.Exception;
            }

            string? refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//"refresh_token"
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                Address = discoDocResponse.RevocationEndpoint,
                ClientId = _clientConfiguration.WebClientForUsers.ClientId,
                ClientSecret = _clientConfiguration.WebClientForUsers.ClientSecret,
                Token = refreshToken,
                TokenTypeHint="refresh_token"
            };
            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SignInVM signInVM)
        {
            ///endpoints :
            DiscoveryDocumentResponse discoDocResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceAPIConfiguration.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }

            });
            if (discoDocResponse.IsError)
            {
                throw discoDocResponse.Exception;
            }
            
            PasswordTokenRequest passwordTokenRequest = new PasswordTokenRequest
            {

                ClientId = _clientConfiguration.WebClientForUsers.ClientId,
                ClientSecret = _clientConfiguration.WebClientForUsers.ClientSecret,
                UserName = signInVM.Email,
                Password = signInVM.Password,
                Address = discoDocResponse.TokenEndpoint
            };

            TokenResponse token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            if (token.IsError)
            {
                string responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                ErrorDto? errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Response<bool>.Fail(errorDto.Errors, 400);
            }

            UserInfoRequest userInfoRequest = new()
            {
                Token = token.AccessToken,
                Address = discoDocResponse.UserInfoEndpoint
            };

            UserInfoResponse userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity = new(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            ClaimsPrincipal claimsPrinciple = new(claimsIdentity);

            AuthenticationProperties authenticationProperties = new();
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
                new AuthenticationToken{ Name= OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture) }
            });
            authenticationProperties.IsPersistent = signInVM.IsRemember;

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimsPrinciple,authenticationProperties);

            return Response<bool>.Success(200);

        }
    }
}
