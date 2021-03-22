using System.Threading.Tasks;
using PMAuth.Models.OAuthUniversal;
#pragma warning disable 1591

namespace PMAuth.Services.Abstract
{
    public interface IProfileManager<T> where T : TokenModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokensModel"></param>
        /// <returns></returns>
        Task<UserProfile> GetUserProfileAsync(T tokensModel);
    }
}