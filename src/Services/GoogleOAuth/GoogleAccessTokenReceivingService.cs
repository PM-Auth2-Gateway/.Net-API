using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
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

        private readonly IExchangeCodeForTokenService exchangeCodeForTokenService;
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GoogleAccessTokenReceivingService> _logger;

        public GoogleAccessTokenReceivingService(
            IExchangeCodeForTokenService exchangeCodeForTokenService,
            BackOfficeContext context,
            IMemoryCache memoryCache,
            ILogger<GoogleAccessTokenReceivingService> logger)
        {
            this.exchangeCodeForTokenService = exchangeCodeForTokenService;
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
        }
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
                {"scope", string.Empty},
                {"grant_type", "authorization_code"}
            };

            string tokenUri = _context.Socials.FirstOrDefault(s => s.Id == sessionInformation.SocialId)?.TokenUrl;


            string responseBody = await exchangeCodeForTokenService.ExchangeCodeForTokens(tokenUri, queryParams, SocialServiceName);
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

    }
}