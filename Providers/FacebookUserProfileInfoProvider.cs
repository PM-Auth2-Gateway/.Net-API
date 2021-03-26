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
        //    var dic = facebookInfo.AdditionalInformation;
        //    dic.Add("location", facebookInfo.Location["location"]);
            return new UserProfile()
            {
                Id = facebookInfo.Id,
                FirstName = facebookInfo.FirstName,
                LastName = facebookInfo.LastName,
                Name = facebookInfo.FullName,
                Email = facebookInfo.Email,
               // Location = facebookInfo.Location,
                AdditionalInformation = facebookInfo.AdditionalInformation
            };
        }
    }
}
