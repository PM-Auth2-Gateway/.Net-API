using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMAuth.Models
{
    /// <summary>
    /// Social Link parameters
    /// </summary>
    public class SocialLinkModel
    {
        /// <summary>
        /// Auth Uri.
        /// </summary>
        [JsonPropertyName("auth_uri")]
        public string AuthUri { get; set; }

        /// <summary>
        /// Prompt.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        /// <summary>
        /// Response Type.
        /// </summary>
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }

        /// <summary>
        /// Client Id.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Scope.
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Access Type.
        /// </summary>
        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

    }
}
