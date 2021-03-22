using System.Text.Json.Serialization;

namespace PMAuth.Models.OAuthUniversal
{
    public class TokenModel
    {
        [JsonPropertyName("access_token")]
        public virtual string AccessToken { get; set; }
        
        [JsonPropertyName("expires_in")]
        public virtual int ExpiresIn { get; set; }

    }
}