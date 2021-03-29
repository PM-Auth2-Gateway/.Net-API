using Microsoft.Extensions.Logging;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Extensions;
using PMAuth.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace PMAuth.Services.OAuthUniversal
{
    public class ExchangeCodeForTokenService : IExchangeCodeForTokenService
    {
        private readonly ILogger<ExchangeCodeForTokenService> logger;
        private readonly HttpClient httpClient;

        public ExchangeCodeForTokenService(
            IHttpClientFactory httpClientFactory,
            ILogger<ExchangeCodeForTokenService> logger)
        {
            this.logger = logger;

            httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> ExchangeCodeForTokens(
            string tokenUri, 
            IDictionary<string, string> queryParams, 
            string socialName)
        {

            if (string.IsNullOrEmpty(tokenUri))
            {
                logger.LogError("Token uri is null or empty");

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
            };

            Uri requestUri = new Uri(tokenUri);
            foreach (KeyValuePair<string, string> queryParam in queryParams)
            {
                requestUri = requestUri.AddQuery(queryParam.Key, queryParam.Value);
            }

            httpRequestMessage.RequestUri = requestUri;

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException exception)
            {
                logger.LogError("The request failed due to an underlying issue such as " +
                                   "network connectivity, DNS failure, server certificate validation or timeout");
                throw new AuthorizationCodeExchangeException($"Unable to retrieve response from the {socialName}", exception);
            }

            try
            {
                response.EnsureSuccessStatusCode(); // throws HttpRequestException if StatusCode unsuccessful
            }
            catch (HttpRequestException exception)
            {
                logger.LogError($"Response StatusCode from the {socialName} " +
                                 "is unsuccessful when trying to exchange code for tokens");
                throw await HandleUnsuccessfulStatusCode(response, exception, socialName);
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<AuthorizationCodeExchangeException> HandleUnsuccessfulStatusCode(
            HttpResponseMessage response, 
            HttpRequestException exception,
            string socialName)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var errorExplanation = JsonSerializer.Deserialize<ErrorModel>(responseBody);
                return new AuthorizationCodeExchangeException($"Response StatusCode from the {socialName} " +
                                                              "is unsuccessful when trying to exchange code " +
                                                              "for tokens", errorExplanation, exception);
            }
            catch (Exception jsonException)
            {
                if (jsonException is ArgumentNullException ||
                    jsonException is JsonException)
                {
                    logger.LogError("Unable to deserialize error response\n" +
                                     $"Response body: {responseBody}");

                    return new AuthorizationCodeExchangeException($"Response StatusCode from the {socialName} " +
                                                                  "is unsuccessful when trying to exchange code " +
                                                                  "for tokens", exception);
                }
                throw;
            }
        }
    }
}
