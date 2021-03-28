using PMAuth.Models.OAuthFacebook;
using PMAuth.Models.OAuthUniversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Providers
{
    public static class FacebookUserProfileInfoProvider
    {
        public static UserProfile Provider(FacebookInfoModel facebookInfo)
        {
            return new UserProfile()
            {
                Id = facebookInfo.Id,
                FirstName = facebookInfo.FirstName,
                LastName = facebookInfo.LastName,
                Name = facebookInfo.FullName,
                Email = facebookInfo.Email,
                AdditionalInformation = facebookInfo.AdditionalInformation
            };
        }
    }
}
