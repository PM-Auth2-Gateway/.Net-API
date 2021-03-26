using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Extensions;
using PMAuth.Models.OAuthGoogle;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Services.Abstract;

#pragma warning disable 1591

namespace PMAuth.Services.GoogleOAuth
{
    public class GoogleAccessTokenReceivingService : IAccessTokenReceivingService
    {
        public string SocialServiceName => "google";
        
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GoogleAccessTokenReceivingService> _logger;
        private readonly HttpClient _httpClient;

        public GoogleAccessTokenReceivingService(
            IHttpClientFactory httpClientFactory, 
            BackOfficeContext context,
            IMemoryCache memoryCache,
            ILogger<GoogleAccessTokenReceivingService> logger) 
        {
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<TokenModel> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            string responseBody = await ExchangeCodeForTokens(appId, authorizationCodeModel);
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return null;
            }
            
            try
            {
                GoogleTokensModel tokens = JsonSerializer.Deserialize<GoogleTokensModel>(responseBody);
                return tokens;
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException ||
                    exception is JsonException)
                {
                    _logger.LogError($"Unable to deserialize response body. Received response body:\n{responseBody}");
                    return null;
                }
                throw;
            }
        }
        
        protected async Task<string> ExchangeCodeForTokens(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            bool isSuccess =
                _memoryCache.TryGetValue(authorizationCodeModel.SessionId, out CacheModel sessionInformation);
            if (isSuccess == false)
            {
                _logger.LogError("Unable to find session id in memory cache." +
                                 "Authorization timeout has expired");
                var errorExplanation = new ErrorModel
                {
                    Error = "Authorization timeout has expired.",
                    ErrorDescription = "Try again later"
                };
                throw new AuthorizationCodeExchangeException(errorExplanation);
            }
            
            // if you need this code, think about moving it to another class
            string tokenUri = _context.Socials.FirstOrDefault(s => s.Id == sessionInformation.SocialId)?.TokenUrl; //"https://oauth2.googleapis.com/token"; 
            string code = authorizationCodeModel.AuthorizationCode;
            string redirectUri = sessionInformation.RedirectUri; // "https://localhost:5001/auth/google";
            /*string clientId = _context.Settings.FirstOrDefault(s => s.AppId == appId && 
                                                               s.SocialId == sessionInformation.SocialId)
                                                               ?.ClientId;*/
            string clientId = "532364683542-3hg1fdiptik9lhbj22o72rrnsb9eqtvi.apps.googleusercontent.com";
            /*string clientSecret = _context.Settings.FirstOrDefault(s => s.AppId == appId &&
                                                                s.SocialId == sessionInformation.SocialId)
                                                                ?.SecretKey;*/
            string clientSecret = "zwqbWaKdRhyyQf6scdJWTiod";

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                _logger.LogError("Client id or client secret is null or empty");
                var errorExplanation = new ErrorModel
                {
                    Error = "Trying to use unregistered social network",
                    ErrorDescription = "There is no client id or client secret key registered in our widget."
                };
                throw new AuthorizationCodeExchangeException(errorExplanation);
            }

            if (string.IsNullOrEmpty(tokenUri))
            {
                _logger.LogError("Token uri is null or empty");

                var errorExplanation = new ErrorModel
                {
                    Error = "Trying to use unregistered social network",
                    ErrorDescription = "This social service is not currently available"
                };
                
                throw new AuthorizationCodeExchangeException(errorExplanation);
            }
            
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(tokenUri)
                    .AddQuery("code", code)
                    .AddQuery("redirect_uri", redirectUri)
                    .AddQuery("client_id", clientId)
                    .AddQuery("client_secret", clientSecret)
                    .AddQuery("scope", string.Empty)
                    .AddQuery("grant_type", "authorization_code")
            };
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError("The request failed due to an underlying issue such as " +
                                   "network connectivity, DNS failure, server certificate validation or timeout");
                throw new AuthorizationCodeExchangeException("Unable to retrieve response from the Google", exception);
            }

            try
            {
                response.EnsureSuccessStatusCode(); // throws HttpRequestException if StatusCode unsuccessful
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError("Response StatusCode from the Google " +
                                 "is unsuccessful when trying to exchange code for tokens");
                throw await HandleUnsuccessfulStatusCode(response, exception);
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<AuthorizationCodeExchangeException> HandleUnsuccessfulStatusCode(HttpResponseMessage response, HttpRequestException exception)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var errorExplanation = JsonSerializer.Deserialize<ErrorModel>(responseBody);
                return new AuthorizationCodeExchangeException("Response StatusCode from the Google " +
                                                              "is unsuccessful when trying to exchange code " +
                                                              "for tokens", errorExplanation, exception);
            }
            catch (Exception jsonException)
            {
                if (jsonException is ArgumentNullException ||
                    jsonException is JsonException)
                {
                    _logger.LogError("Unable to deserialize error response\n" +
                                     $"Response body: {responseBody}");
                    
                    return new AuthorizationCodeExchangeException("Response StatusCode from the Google " +
                                                                  "is unsuccessful when trying to exchange code " +
                                                                  "for tokens", exception);
                }
                throw;
            }
        }
    }
}