using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

using PMAuth.Models.OAuthGoogle;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.Abstract;
#pragma warning disable 1591

namespace PMAuth.Services.GoogleOAuth
{
    public class GoogleProfileManager : IProfileManagingService
    {
        public string SocialServiceName => "google";

        private readonly IMemoryCache _memoryCache;

        public GoogleProfileManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task GetUserProfileAsync(TokenModel rawTokenModel, string sessionId)
        {
            GoogleTokensModel tokensModel = (GoogleTokensModel) rawTokenModel;
            /*if (string.IsNullOrWhiteSpace(tokensModel.IdToken))
            {
                return await GetProfileFromAPICall(tokensModel);
            }*/

            if (tokensModel == null)
            {
                return;
            }
            
            UserProfile profile = GetProfileFromIdToken(tokensModel);
            bool isSuccess = _memoryCache.TryGetValue(sessionId, out CacheModel model);
            if (isSuccess && model != null)
            {
                model.UserProfile = profile;
            }
        }
        
        /*private async Task<UserProfile> GetProfileFromAPICall(GoogleTokensModel tokensModel)
        {
            // here can be awaitable code, so GetUserProfileAsync should stay async for now
            return null;
        }*/
        
        private UserProfile GetProfileFromIdToken(GoogleTokensModel tokensModel)
        {
            var jwtEncodedString = tokensModel.IdToken;

            var token = new JwtSecurityToken(jwtEncodedString);
            UserProfile userProfile = new UserProfile
            {
                Id = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                //AccessToken = tokensModel.AccessToken,
                //RefreshToken = tokensModel.RefreshToken,
                //ExpiresIn = tokensModel.ExpiresIn,
                Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                FirstName = token.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                LastName = token.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                Photo = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                Locale = token.Claims.FirstOrDefault(c => c.Type == "locale")?.Value,
            };

            string isVerifiedEmailString = token.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
            if (!string.IsNullOrWhiteSpace(isVerifiedEmailString) && bool.TryParse(isVerifiedEmailString, out bool isVerifiedBool))
            {
                userProfile.IsVerifiedEmail = isVerifiedBool;
            }

            return userProfile;
        }
    }
}