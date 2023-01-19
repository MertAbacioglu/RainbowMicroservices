using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Settings;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace FreeCourse.Web.Services.Concretes
{
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        private readonly ServiceApiConfiguration _serviceApiConfiguration;
        private readonly ClientConfiguration _clientConfiguration;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly HttpClient _httpClient;

        public ClientCredentialTokenService(IOptions<ServiceApiConfiguration> serviceApiConfiguration, IOptions<ClientConfiguration> clientConfiguration, IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
        {
            _serviceApiConfiguration = serviceApiConfiguration.Value;
            _clientConfiguration = clientConfiguration.Value;
            _clientAccessTokenCache = clientAccessTokenCache;
            _httpClient = httpClient;
        }

        public async Task<string> GetToken()
        {
            ClientAccessToken? currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken");
            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }
            DiscoveryDocumentResponse discoDocResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiConfiguration.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }

            });
            if (discoDocResponse.IsError)
            {
                throw discoDocResponse.Exception;
            }

            ClientCredentialsTokenRequest tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discoDocResponse.TokenEndpoint,
                ClientId = _clientConfiguration.WebClient.ClientId,
                ClientSecret = _clientConfiguration.WebClient.ClientSecret
            };

            TokenResponse newToken = await _httpClient.RequestClientCredentialsTokenAsync(tokenRequest);
            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn);
            return newToken.AccessToken;
        }
    }
}
