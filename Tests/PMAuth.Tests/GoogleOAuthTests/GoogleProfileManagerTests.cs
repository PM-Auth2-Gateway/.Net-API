using Microsoft.Extensions.Caching.Memory;
using PMAuth.Models.OAuthGoogle;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.GoogleOAuth;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using PMAuth.Tests.Utilities;

namespace PMAuth.Tests.GoogleOAuthTests
{
    public class GoogleProfileManagerTests
    {
        private string _validTestSessionId = "123";
        TokenModel _validTestTokenModel = new GoogleTokensModel
        {
            IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEzZThkNDVhNDNjYjIyNDIxNTRjN2Y0ZGFmYWMyOTMzZmVhMjAzNzQiLCJ0e" +
                          "XAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI0MDc0MDg3MTgxOTI" +
                          "uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI0MDc0MDg3MTgxOTIuYXBwcy5nb29nbGV1c2VyY29u" +
                          "dGVudC5jb20iLCJzdWIiOiIxMTgwMTQwMDE3NjQ3MTM1NTM1OTEiLCJlbWFpbCI6InR1dHVrb2tvamoxMjNAZ21ha" +
                          "WwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJDd0pMZG5Ybm5iS3pxS2kxYzZzWVlnIiwibm" +
                          "FtZSI6ImZ5dmdqdmdqdiBiamJqYmJqIiwicGljdHVyZSI6Imh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmN" +
                          "vbS8tb18yT0tHNlA0bUUvQUFBQUFBQUFBQUkvQUFBQUFBQUFBQUEvQU1adXVjbTNQWHJyUXFXZlI1RnNzV1NhbVJY" +
                          "R0JiVUc5US9zOTYtYy9waG90by5qcGciLCJnaXZlbl9uYW1lIjoiZnl2Z2p2Z2p2IiwiZmFtaWx5X25hbWUiOiJia" +
                          "mJqYmJqIiwibG9jYWxlIjoicnUiLCJpYXQiOjE2MTY5NDE5NjQsImV4cCI6MTYxNjk0NTU2NH0.bD0EX7yJt7hLes" +
                          "AfT0gT5DGBfBzKcwgdTHJvFK90MynrBgSB1-azSI8GmvKm-3SIIHNAEJG_5l0w2558OAecbGc87m-kKDxtsO7Xb8S" +
                          "v8_VxYRk005CnRMxygqzyDnCc38PUiY691fEAnoXffjq-ll3sZ-K-QHqb873K8EDptH_0IdGNn_XmfaBaeJhQgxYT" +
                          "WIvFurtIFdqh4MRQnCaBE6BhCTCfvKQ7Kto4mIYC_-7F3Lgda26jfQ_TpAXyG8Lf2XzKOEVUMboFIZz0bBetVgBlG" +
                          "ScSIIDfhLUZ4qLLo5goHTT_gCWM4RNIqKMeA36bp95Xt_0dMWNGfC7c3Muk3Q"
        };

        private IMemoryCache _memoryCacheMock;
        private GoogleProfileManager _testingProfileManager;

        public GoogleProfileManagerTests()
        {
            CacheModel testCacheModel = new CacheModel
            {
                UserProfile = null
            };

            _memoryCacheMock = MockMemoryCacheService.GetMemoryCache(_validTestSessionId, testCacheModel);
            _testingProfileManager = new GoogleProfileManager(_memoryCacheMock);
        }

        [Fact]
        public async Task GetUserProfile_ValidParameters_UserProfileAddedInMemoryCache()
        {
            //arrange          
            object expectedUserProfile = new UserProfile
            {
                Id = "118014001764713553591",
                FirstName = "fyvgjvgjv",
                LastName = "bjbjbbj",
                Email = "tutukokojj123@gmail.com",
                IsVerifiedEmail = true,
                Photo = "https://lh5.googleusercontent.com/-o_2OKG6P4mE/AAAAAAAAAAI/AAAAAAAAAAA/AMZuucm3PXrrQqWfR5FssWSamRXGBbUG9Q/s96-c/photo.jpg",
                Locale = "ru",
            };

            //act
            await _testingProfileManager.GetUserProfileAsync(_validTestTokenModel, _validTestSessionId);
            UserProfile actualUserProfile = _memoryCacheMock.Get<CacheModel>(_validTestSessionId)?.UserProfile;

            //assert
            expectedUserProfile.Should().BeEquivalentTo(actualUserProfile);
        }


        [Fact]
        public async Task GetUserProfile_NullTokenModelValidSessionId_UserProfileInMemoryCacheIsEmpty()
        {
            //arrange
            TokenModel testTokenModel = null;

            //act
            await _testingProfileManager.GetUserProfileAsync(testTokenModel, _validTestSessionId);
            UserProfile actualUserProfile = _memoryCacheMock.Get<CacheModel>(_validTestSessionId)?.UserProfile;

            //assert
            actualUserProfile.Should().BeNull();
        }

        [Fact]
        public async Task GetUserProfile_ValidTokenModelNullSessionId_UserProfileInMemoryCacheIsEmpty()
        {
            //arrange
            string testSessionId = null;

            //act
            await _testingProfileManager.GetUserProfileAsync(_validTestTokenModel, testSessionId);
            UserProfile actualUserProfile = _memoryCacheMock.Get<CacheModel>(testSessionId)?.UserProfile;

            //assert
            actualUserProfile.Should().BeNull();
        }

        [Fact]
        public async Task GetUserProfile_NullTokenModelNullSessionId_UserProfileInMemoryCacheIsEmpty()
        {
            //arrange
            TokenModel testTokenModel = null;
            string testSessionId = null;

            //act
            await _testingProfileManager.GetUserProfileAsync(testTokenModel, testSessionId);
            UserProfile actualUserProfile = _memoryCacheMock.Get<CacheModel>(testSessionId)?.UserProfile;

            //assert
            actualUserProfile.Should().BeNull();
        }

        [Fact]
        public async Task GetUserProfile_UnexistingSessionId_NoExceptionThrown()
        {
            //arrange
            string unexistingSessionId = "1";
            CacheModel testCacheModel = new CacheModel
            {
                UserProfile = null
            };

            _memoryCacheMock = MockMemoryCacheService.GetMemoryCache(unexistingSessionId, testCacheModel);

            //act
            await _testingProfileManager.GetUserProfileAsync(_validTestTokenModel, unexistingSessionId);
            UserProfile actualUserProfile = _memoryCacheMock.Get<CacheModel>(unexistingSessionId)?.UserProfile;
            UserProfile userProfileForValidSession = _memoryCacheMock.Get<CacheModel>(_validTestSessionId)?.UserProfile;

            //assert
            actualUserProfile.Should().BeNull();
            userProfileForValidSession.Should().BeNull();
        }

    }

}
