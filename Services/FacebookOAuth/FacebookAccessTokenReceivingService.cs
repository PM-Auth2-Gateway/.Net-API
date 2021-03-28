using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Extensions;
using PMAuth.Models.OAuthFacebook;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Services.Abstract;
#pragma warning disable CS1591

namespace PMAuth.Services.FacebookOAuth
{
    /// <summary>
    /// Service that operates Facebook Access token
    /// </summary>
    public class FacebookAccessTokenReceivingService : IAccessTokenReceivingService
    {
        public string SocialServiceName => "facebook";

        private readonly BackOfficeContext _context;

        private readonly IMemoryCache _memoryCache;

        private readonly ILogger<FacebookAccessTokenReceivingService> _logger;

        private readonly HttpClient _httpClient;

        public FacebookAccessTokenReceivingService(
            IHttpClientFactory httpClientFactory, 
            BackOfficeContext context,
            IMemoryCache memoryCache,
            ILogger<FacebookAccessTokenReceivingService> logger) 
        {
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Method that exchange user authorization code for tokens
        /// </summary>
        /// <param name="appId">Application Id</param>
        /// <param name="authorizationCodeModel">
        /// Authorization model that has code, scope and state
        /// </param>
        /// <returns>Tokens model</returns>
        public async Task<TokenModel> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            string responseBody = await ExchangeCodeForTokens(appId, authorizationCodeModel);
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return null;
            }
            
            try
            {
                FacebookTokensModel tokens = JsonSerializer.Deserialize<FacebookTokensModel>(responseBody);
                tokens.Scope = authorizationCodeModel.Scope;
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
                var errorExplanation = new ErrorModel
                {
                    Error = "Authorization timeout has expired.",
                    ErrorDescription = "Try again later"
                };
                _logger.LogError("Unable to find session id in memory cache." +
                                 "Authorization timeout has expired");
                throw new AuthorizationCodeExchangeException(errorExplanation);
            }
            
            string tokenUri = _context.Socials.FirstOrDefault(s => s.Id == sessionInformation.SocialId)?.TokenUrl;

            string code = authorizationCodeModel.AuthorizationCode;

            string redirectUri = sessionInformation.RedirectUri;

            Setting setting = _context.Settings.FirstOrDefault(
                    s => s.AppId == appId &&s.SocialId == sessionInformation.SocialId);

            string clientId = setting?.ClientId;

            string clientSecret = setting?.SecretKey;
            

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
                
                throw new AuthorizationCodeExchangeException("Unable to retrieve response from the Facebook", exception);
            }

            try
            {
                response.EnsureSuccessStatusCode(); // throws HttpRequestException if StatusCode unsuccessful
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError("Response StatusCode from the Facebook " +
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
                return new AuthorizationCodeExchangeException("Response StatusCode from the Facebook " +
                                                              "is unsuccessful when trying to exchange code " +
                                                              "for tokens", errorExplanation, exception);
            }
            catch (Exception jsonException)
            {
                if (jsonException is ArgumentNullException ||
                    jsonException is JsonException)
                {
                    _logger.LogError("Response StatusCode from the Facebook is unsuccessful " +
                                     "when trying to exchange code for tokens." +
                                     "Unable to deserialize error response\n" +
                                     $"Response body: {responseBody}");
                    return new AuthorizationCodeExchangeException("Response StatusCode from the Facebook " +
                                                                  "is unsuccessful when trying to exchange code " +
                                                                  "for tokens", exception);
                }
                throw;
            }
        }
    }
}