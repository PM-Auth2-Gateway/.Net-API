using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PMAuth.Extensions;
using PMAuth.Models.OAuthFacebook;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Providers;
using PMAuth.Services.Abstract;
#pragma warning disable 1591

namespace PMAuth.Services.FacebookOAuth
{
    /// <summary>
    /// Managing user's Facebook profile information 
    /// - get user info via tokens
    /// </summary>
    public class FacebookProfileManager : IProfileManagingService
    {
        public string SocialServiceName => "facebook";

        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly FacebookProperties facebookProperties;

        public FacebookProfileManager(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClient = httpClientFactory.CreateClient();

            var json = File.ReadAllText("facebookProperties.json");
            facebookProperties = JsonSerializer.Deserialize<FacebookProperties>(json);
        }

        public async Task GetUserProfileAsync(TokenModel rawTokenModel, string sessionId)
        {
            FacebookTokensModel tokensModel = (FacebookTokensModel) rawTokenModel;

            if (tokensModel == null)
            {
                return;
            }
            
            UserProfile profile = await GetProfileFromAccessTokenAsync(tokensModel);

            if (profile == null)
            {
                return;
            }

            bool isSuccess = _memoryCache.TryGetValue(sessionId, out CacheModel model);
            if (isSuccess && model != null)
            {
                model.UserProfile = profile;
            }
        }


        private async Task<UserProfile> GetProfileFromAccessTokenAsync(FacebookTokensModel tokensModel)
        {
            string fields = ReformScopeToFields.Transform(tokensModel.Scope);
            
            string url = facebookProperties.GetProfileLink + tokensModel.AccessToken +
                    "&fields=first_name,last_name,name,gender,location" + fields;

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
            } catch (HttpRequestException)
            {
                return null;
            }

            string facebookInfoJson = await response.Content.ReadAsStringAsync();

            FacebookInfoModel facebookInfo = JsonSerializer.Deserialize<FacebookInfoModel>(facebookInfoJson);

            return FacebookUserProfileInfoProvider.Provider(facebookInfo); 
        }
    }
}