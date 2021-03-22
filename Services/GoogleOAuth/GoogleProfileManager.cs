using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using PMAuth.Models.OAuthGoogle;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.Abstract;
#pragma warning disable 1591

namespace PMAuth.Services.GoogleOAuth
{
    public class GoogleProfileManager : IProfileManager<TokenModel>
    {
        public async Task<UserProfile> GetUserProfileAsync(TokenModel rawTokenModel)
        {
            GoogleTokensModel tokensModel = (GoogleTokensModel) rawTokenModel;
            if (string.IsNullOrWhiteSpace(tokensModel.IdToken))
            {
                return await GetProfileFromAPICall(tokensModel);
            }
            else
            {
                UserProfile profile = GetProfileFromIdToken(tokensModel);
                return profile;
            }
        }
        
        private async Task<UserProfile> GetProfileFromAPICall(GoogleTokensModel tokensModel)
        {
            // here can be awaitable code, so GetUserProfileAsync should stay async for now
            //TODO add variant of getting user profile via Google OAuth API
            return null;
        }
        
        private UserProfile GetProfileFromIdToken(GoogleTokensModel tokensModel)
        {
            var jwtEncodedString = tokensModel.IdToken;

            var token = new JwtSecurityToken(jwtEncodedString);
            UserProfile userProfile = new UserProfile
            {
                Id = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                AccessToken = tokensModel.AccessToken,
                RefreshToken = tokensModel.RefreshToken,
                ExpiresIn = tokensModel.ExpiresIn,
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


    /*
     public class GoogleProfileManager : IProfileManager<TokenModel>
    {
        public Task<UserProfile> GetUserProfileAsync(TokenModel rawTokenModel)
        {
            GoogleTokensModel tokensModel = (GoogleTokensModel) rawTokenModel;
        }
    }
    */
}