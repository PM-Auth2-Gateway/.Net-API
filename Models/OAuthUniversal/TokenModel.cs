using System.Text.Json.Serialization;

namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Base token model. Can be used for multiple social networks
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Access token which allows us to make calls to social network API
        /// </summary>
        [JsonPropertyName("access_token")]
        public virtual string AccessToken { get; set; }
        
        /// <summary>
        /// Expiration time of token in seconds
        /// </summary>
        [JsonPropertyName("expires_in")]
        public virtual int ExpiresIn { get; set; }

    }
}