﻿using System.Threading.Tasks;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.Abstract;
#pragma warning disable 1591

namespace PMAuth.Services.OAuthUniversal
{
    public class UserProfileReceivingServiceContext : IUserProfileReceivingServiceContext
    {
        private IAccessTokenReceivingService _accessTokenReceivingStrategy;
        private IProfileManager _profileManagerStrategy;

        public void SetStrategies(
            IAccessTokenReceivingService accessTokenReceivingStrategy,
            IProfileManager profileManagerStrategy)
        {
            _accessTokenReceivingStrategy = accessTokenReceivingStrategy;
            _profileManagerStrategy = profileManagerStrategy;
        }


        public async Task<UserProfile> Execute(int appId, AuthorizationCodeModel authorizationCodeModel)
        {
            TokenModel tokens = await _accessTokenReceivingStrategy.ExchangeAuthorizationCodeForTokens(appId, authorizationCodeModel);
            UserProfile userProfile = await _profileManagerStrategy.GetUserProfileAsync(tokens);
            return userProfile;
        }
    }
}