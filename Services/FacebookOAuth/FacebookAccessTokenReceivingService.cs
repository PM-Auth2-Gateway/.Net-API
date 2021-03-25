﻿using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Extensions;
using PMAuth.Models.OAuthFacebook;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Services.Abstract;

#pragma warning disable 1591

namespace PMAuth.Services.FacebookOAuth
{
    public class FacebookAccessTokenReceivingService : IAccessTokenReceivingService
    {
        private readonly BackOfficeContext _context;
        //private readonly ILogger<GoogleAccessTokenReceivingService> _logger;
        private readonly HttpClient _httpClient;

        public FacebookAccessTokenReceivingService(
            IHttpClientFactory httpClientFactory, 
            BackOfficeContext context
            /*ILogger<GoogleAccessTokenReceivingService> logger*/) 
        {
            _context = context;
            //_logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<TokenModel> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            string responseBody = await SendRequest(appId, authorizationCodeModel);
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return null;
            }
            
            try
            {
                FacebookTokensModel tokens = JsonSerializer.Deserialize<FacebookTokensModel>(responseBody);
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
        
        protected async Task<string> SendRequest(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            // if you need this code, think about moving it to another class
            string tokenUri = "https://graph.facebook.com/v10.0/oauth/access_token";  //TODO should be added to database. for now it is hardcoded
            string code = authorizationCodeModel.AuthorizationCode;
            string redirectUri = "https://localhost:44313/auth/facebook";//authorizationCodeModel.RedirectUri;
            /*string clientId = _context.Settings.FirstOrDefault(s => s.AppId == appId && 
                                                               s.SocialId == authorizationCodeModel.SocialId)
                                                               ?.ClientId;*/
            string clientId = "929331344507670";
            /*string clientSecret = _context.Settings.FirstOrDefault(s => s.AppId == appId &&
                                                                s.SocialId == authorizationCodeModel.SocialId)
                                                                ?.SecretKey;*/
            string clientSecret = "164d0e742d123c6547c641d9393b865c";

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                var errorExplanation = new ErrorModel
                {
                    Error = "Secrets are unavailable",
                    ErrorDescription = "There is no client id or client secret key registered in our widget."
                };
                throw new AuthorizationCodeExchangeException("Error. Trying to use unregistered social network", errorExplanation);
            }
            
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri($"{tokenUri}?code={code}&redirect_uri={redirectUri}&client_id={clientId}&client_secret={clientSecret}&scope=&grant_type=authorization_code"),
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
                //_logger.LogInformation("Unable to retrieve response from the Google");
                throw new AuthorizationCodeExchangeException("Unable to retrieve response from the Facebook", exception);
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
                    return new AuthorizationCodeExchangeException("Response StatusCode from the Facebook " +
                                                                  "is unsuccessful when trying to exchange code " +
                                                                  "for tokens", exception);
                }
                throw;
            }
        }
    }
}