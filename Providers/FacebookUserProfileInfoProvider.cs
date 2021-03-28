using PMAuth.Models.OAuthFacebook;
using PMAuth.Models.OAuthUniversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Providers
{
    /// <summary>
    /// Provider from FacebookInfoModel to UserProfile
    /// </summary>
    public static class FacebookUserProfileInfoProvider
    {
        /// <summary>
        /// Mapps UserProfile fields
        /// </summary>
        /// <param name="facebookInfo">FacebookInfoModel</param>
        /// <returns>UserProfile</returns>
        public static UserProfile Provider(FacebookInfoModel facebookInfo)
        {
            if(facebookInfo == null)
            {
                return null;
            }

            return new UserProfile()
            {
                Id = facebookInfo.Id,
                FirstName = facebookInfo.FirstName,
                LastName = facebookInfo.LastName,
                Name = facebookInfo.FullName,
                Email = facebookInfo.Email,
                Photo = facebookInfo?.Pricture?.Data?.Url,
                AdditionalInformation = facebookInfo.AdditionalInformation
            };
        }
    }
}
