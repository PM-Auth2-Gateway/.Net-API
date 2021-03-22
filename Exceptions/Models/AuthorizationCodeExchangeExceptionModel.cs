using System.Text.Json.Serialization;

namespace PMAuth.Exceptions.Models
{
    /// <summary>
    /// Error model. It will be put in response if something went wrong while getting user profile from social network
    /// </summary>
    public class AuthorizationCodeExchangeExceptionModel
    {
        /// <summary>
        /// Detailed description of an error
        /// </summary>
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}