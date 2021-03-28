using System.Threading.Tasks;
using PMAuth.Exceptions;
using PMAuth.Models.OAuthUniversal.RedirectPart;

namespace PMAuth.Services.Abstract
{
    /// <summary>
    /// Context for setting code flow for processing exchange of authorization code on user profile
    /// Consists of two strategies.
    /// 1. Strategy of exchanging authorization code for access token (IAccessTokenReceivingService)
    ///     1.1 All exceptions that occurs during the process of code exchange should be wrapped into AuthorizationCodeExchangeException
    /// 2. Strategy of getting user profile from the social network and returning it in unified model (UserProfile)
    ///     2.1 If on this stage you need to make calls to the social network API, exception during these calls should be also
    ///     wrapped into AuthorizationCodeExchangeException.
    ///     2.2 If there may be some exceptions regarding models mapping (like JsonException), contact your teammates and
    ///     introduce a new type of exception to avoid leaky abstraction
    /// </summary>
    public interface IUserProfileReceivingServiceContext
    {
        /// <summary>
        /// Set code flow for chosen social network
        /// </summary>
        /// <param name="accessTokenReceivingStrategy">service which exchanges authorization token on access token</param>
        /// <param name="profileManagerStrategy">service which gets user profile from social network and return unified user profile</param>
        void SetStrategies(
            IAccessTokenReceivingService accessTokenReceivingStrategy,
            IProfileManagingService profileManagerStrategy);
        
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
        Task Execute(int appId, AuthorizationCodeModel authorizationCodeModel);
    }
}