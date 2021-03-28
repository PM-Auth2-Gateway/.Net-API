using System.Text.Json.Serialization;
using PMAuth.Models.OAuthUniversal;

namespace PMAuth.Models.OAuthFacebook
{
    /// <summary>
    /// Model for retrieving access token and scope from Facebook 
    /// </summary>
    public class FacebookTokensModel : TokenModel
    {
        /// <summary>
        /// Token type, always Bearer
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Used scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

    }
}