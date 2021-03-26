using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Unified model of user profile
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Users unique ID in social network used for authorization
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User's first name received from the social network (may be absent)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name received from the social network (may be absent)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full user's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User's email received from the social network (may be absent)
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Is user's email verified received from the social network (may be absent)
        /// </summary>
        public bool? IsVerifiedEmail { get; set; }

        /// <summary>
        /// User's profile photo link received from the social network (may be absent)
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// User's profile locale received from the social network (may be absent)
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Key-value pairs ("paramName": "value") of additional information about user
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalInformation { get; set; } = new Dictionary<string, object>();
        
    }
}