using System.Threading.Tasks;
using PMAuth.Models.OAuthUniversal;

namespace PMAuth.Services.Abstract
{
    /// <summary>
    /// Managing user's profile information
    /// </summary>
    public interface IProfileManagingService
    {
        public string SocialServiceName { get; }
        /// <summary>
        /// Method gets user profile in registered socials,
        ///     by their tokens and user session id.
        /// </summary>
        /// <param name="tokensModel">
        /// Token model, which contains tokens and may being extends.
        /// </param>
        /// <param name="sessionId">
        /// User session Id that gives when user push the auth button
        /// </param>
        /// <returns>Nothing</returns>
        Task GetUserProfileAsync(TokenModel tokensModel, string sessionId);
    }
}