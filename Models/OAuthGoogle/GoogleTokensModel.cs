using System.Text.Json.Serialization;
using PMAuth.Models.OAuthUniversal;

namespace PMAuth.Models.OAuthGoogle
{
    public class GoogleTokensModel : TokenModel
    {
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}