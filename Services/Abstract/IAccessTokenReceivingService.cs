using System.Threading.Tasks;
using PMAuth.Exceptions;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;

#pragma warning disable 1591

namespace PMAuth.Services.Abstract
{
    public interface IAccessTokenReceivingService // T is a model that contains tokens
    {
        string SocialServiceName { get; }
        /// <summary>
        /// Method for exchanging authorization code for access token
        /// </summary>
        /// <param name="appId">application ID</param>
        /// <param name="authenticationCodeModel">Model with authorization code</param>
        /// <returns>Model with access token and other info</returns>
        /// <exception cref="AuthorizationCodeExchangeException">
        /// The request failed due to an underlying issue such as network connectivity,
        /// DNS failure, server certificate validation, timeout or HTTP response is unsuccessful.
        /// </exception>
        Task<TokenModel> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authenticationCodeModel);
    }
}