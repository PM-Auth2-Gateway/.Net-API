using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Unified model of admin profile
    /// </summary>
    public class AdminProfile:UserProfile
    {
        /// <summary>
        /// Access token what need from authentication
        /// </summary>
        public string Token { get; set; }

        public AdminProfile(UserProfile user)
        {
            Id = user?.Id;
            FirstName = user?.FirstName;
            LastName = user?.LastName;
            Email = user?.Email;
            IsVerifiedEmail = user?.IsVerifiedEmail;
            Photo = user?.Photo;
            Locale = user?.Locale;
            AdditionalInformation = user?.AdditionalInformation;
        }
    }
}
