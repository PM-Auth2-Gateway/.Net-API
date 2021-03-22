using System.Text.Json.Serialization;
using PMAuth.Models.OAuthUniversal;

namespace PMAuth.Models.OAuthGoogle
{
    /// <summary>
    /// Model for retrieving access and refresh tokens from Google 
    /// </summary>
    public class GoogleTokensModel : TokenModel
    {
        /// <summary>
        /// JWT token which contains all user info according to used scope
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

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

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}