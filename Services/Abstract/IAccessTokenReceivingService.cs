using System.Threading.Tasks;
using PMAuth.Models.OAuthUniversal;
#pragma warning disable 1591

namespace PMAuth.Services.Abstract
{
    public interface IAccessTokenReceivingService<T>
    {
        public Task<T> ExchangeAuthorizationCodeForTokens(int appId, AuthorizationCodeModel authenticationCodeModel);
    }
}