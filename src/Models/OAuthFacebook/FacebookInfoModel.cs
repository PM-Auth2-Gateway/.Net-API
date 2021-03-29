using PMAuth.Models.OAuthFacebook.FacebookPicture;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PMAuth.Models.OAuthFacebook
{
    /// <summary>
    /// Facebook user information model on scopes
    /// </summary>
    public class FacebookInfoModel
    {
        /// <summary>
        /// User id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// User first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// User last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Full user's name
        /// </summary>
        [JsonPropertyName("name")]
        public string FullName { get; set; }

        /// <summary>
        /// User registered Email.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Picture profile of user
        /// </summary>
        [JsonPropertyName("picture")]
        public FacebookProfilePicture Pricture { get; set; }

        /// <summary>
        /// Key-value pairs ("paramName": "value") of additional information about user.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalInformation { get; set; } = new Dictionary<string, object>();
    }
}
