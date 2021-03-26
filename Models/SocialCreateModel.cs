using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    public class SocialCreateModel
    {
        /// <summary>
        /// Client Id
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        /// <summary>
        /// Scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        /// <summary>
        /// Secret Key
        /// </summary>

        [JsonPropertyName("secret_key")]
        public string SecretKey { get; set; }
    }
}
