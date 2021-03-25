using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    /// <summary>
    /// Social Link parameters
    /// </summary>
    public class SocialLinkModel
    {
        /// <summary>
        /// Auth Uri - base link of some service which offer authentication.
        /// </summary>
        [JsonPropertyName("auth_uri")]
        public string AuthUri { get; set; }

        /// <summary>
        /// Redirect Uri - on which adress.
        /// </summary>
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        /// <summary>
        /// Response Type - default = code.
        /// </summary>
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }

        /// <summary>
        /// Client Id.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Scope - bunch of accessed information by user.
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// State - session id.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

    }
}
