using System;
using System.Collections.Generic;
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

        private readonly IExchangeCodeForTokenService exchangeCodeForTokenService;

        private readonly BackOfficeContext _context;

        private readonly IMemoryCache _memoryCache;

        private readonly ILogger<FacebookAccessTokenReceivingService> _logger;

        public FacebookAccessTokenReceivingService(
            IExchangeCodeForTokenService exchangeCodeForTokenService,
            BackOfficeContext context,
            IMemoryCache memoryCache,
            ILogger<FacebookAccessTokenReceivingService> logger) 
        {
            this.exchangeCodeForTokenService = exchangeCodeForTokenService;
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
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

            string code = authorizationCodeModel.AuthorizationCode;

            string redirectUri = sessionInformation.RedirectUri;

            string clientId = _context.Settings.FirstOrDefault(s => s.AppId == appId &&
                                                               s.SocialId == sessionInformation.SocialId)?.ClientId;

            string clientSecret = _context.Settings.FirstOrDefault(s => s.AppId == appId &&
                                                               s.SocialId == sessionInformation.SocialId)?.SecretKey;

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

            Dictionary<string, string> queryParams = new Dictionary<string, string>
            {
                {"code" , code},
                {"redirect_uri", redirectUri},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"scope", string.Empty}
            };

            string tokenUri = _context.Socials.FirstOrDefault(s => s.Id == sessionInformation.SocialId)?.TokenUrl;

            string responseBody = await exchangeCodeForTokenService.ExchangeCodeForTokens(tokenUri, queryParams, SocialServiceName);
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
        
    }
}