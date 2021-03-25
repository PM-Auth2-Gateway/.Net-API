using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    public class SocialCreateModel
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("secret_key")]
        public string SecretKey { get; set; }
    }
}
