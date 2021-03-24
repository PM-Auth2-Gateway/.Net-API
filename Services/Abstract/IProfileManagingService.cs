using System.Threading.Tasks;
using PMAuth.Models.OAuthUniversal;
#pragma warning disable 1591

namespace PMAuth.Services.Abstract
{
    public interface IProfileManagingService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokensModel"></param>
        /// <returns></returns>
        Task GetUserProfileAsync(TokenModel tokensModel, string sessionId);
    }
}