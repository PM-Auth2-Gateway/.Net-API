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
        /// Auth Uri.
        /// </summary>
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

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
        /// Scope.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

    }
}
