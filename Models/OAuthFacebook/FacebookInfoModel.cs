using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        /// User full name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// User registered Email.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Key-value pairs ("paramName": "value") of additional information about user.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalInformation { get; set; } = new Dictionary<string, object>();
    }
}
