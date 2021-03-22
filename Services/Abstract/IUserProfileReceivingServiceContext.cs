using System.Threading.Tasks;
using PMAuth.Exceptions;
using PMAuth.Models.OAuthUniversal;

namespace PMAuth.Services.Abstract
{
    public interface IUserProfileReceivingServiceContext
    {
        /// <summary>
        /// Set code flow for chosen social network
        /// </summary>
        /// <param name="accessTokenReceivingStrategy">service which exchanges authorization token on access token</param>
        /// <param name="profileManagerStrategy">service which gets user profile from social network and return unified user profile</param>
        void SetStrategies(
            IAccessTokenReceivingService<TokenModel> accessTokenReceivingStrategy,
            IProfileManager<TokenModel> profileManagerStrategy);

        
        /// <summary>
        /// Get user profile by authorization code
        /// </summary>
        /// <param name="appId">application ID</param>
        /// <param name="authorizationCodeModel">Model with authorization code</param>
        /// <returns>Unified user profile</returns>
        /// <exception cref="AuthorizationCodeExchangeException">
        /// The request failed due to an underlying issue such as network connectivity,
        /// DNS failure, server certificate validation, timeout or HTTP response is unsuccessful.
        /// </exception>
        Task<UserProfile> Execute(int appId, AuthorizationCodeModel authorizationCodeModel);
    }
}