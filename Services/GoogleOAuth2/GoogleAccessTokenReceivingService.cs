using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthGoogle;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.Abstract;

#pragma warning disable 1591

namespace PMAuth.Services.GoogleOAuth2
{
    public class GoogleAccessTokenReceivingService : IAccessTokenReceivingService<GoogleTokensModel>
    {
        private readonly BackOfficeContext _context;
        //private readonly ILogger<GoogleAccessTokenReceivingService> _logger;
        private readonly HttpClient _httpClient;

        public GoogleAccessTokenReceivingService(
            IHttpClientFactory httpClientFactory, 
            BackOfficeContext context
            /*ILogger<GoogleAccessTokenReceivingService> logger*/)
        {
            _context = context;
            //_logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<GoogleTokensModel> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            string responseBody = await SendRequest(appId, authorizationCodeModel);
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
                    return null;
                }
                throw;
            }
        }
        
        private async Task<string> SendRequest(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            string tokenUri = "https://oauth2.googleapis.com/token";  //TODO should be added to database. for now it is hardcoded
            string code = authorizationCodeModel.AuthorizationCode;
            string redirectUri = authorizationCodeModel.RedirectUri;
            string clientId = _context.Settings.FirstOrDefault(s => s.AppId == appId)?.ClientId;
            string clientSecret = _context.Settings.FirstOrDefault(s => s.AppId == appId)?.SecretKey;

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                
            }
            
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{tokenUri}?code={code}&redirect_uri={redirectUri}&client_id={clientId}&client_secret={clientSecret}&scope=&grant_type=authorization_code"),
            };
            
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException exception)
            {
                //_logger.LogInformation("Unable to retrieve response from the Google");
                throw new AuthorizationCodeExchangeException("Unable to retrieve response from the Google", exception);
            }

            try
            {
                response.EnsureSuccessStatusCode(); // throws HttpRequestException if StatusCode unsuccessful
            }
            catch (HttpRequestException exception)
            {
                //_logger.LogInformation("Response StatusCode from the Google is unsuccessful when trying " +
                //                       "to exchange code for tokens");
                throw await HandleUnsuccessfulStatusCode(response, exception); // is it a good practice? 
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<AuthorizationCodeExchangeException> HandleUnsuccessfulStatusCode(HttpResponseMessage response, HttpRequestException exception)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var errorExplanation = JsonSerializer.Deserialize<AuthorizationCodeExchangeExceptionModel>(responseBody);
                return new AuthorizationCodeExchangeException("Response StatusCode from the Google " +
                                                              "is unsuccessful when trying to exchange code " +
                                                              "for tokens", errorExplanation, exception);
            }
            catch (Exception jsonException)
            {
                if (jsonException is ArgumentNullException ||
                    jsonException is JsonException)
                {
                    return new AuthorizationCodeExchangeException("Response StatusCode from the Google " +
                                                                  "is unsuccessful when trying to exchange code " +
                                                                  "for tokens", exception);
                }
                throw;
            }
        }
        
    }
}