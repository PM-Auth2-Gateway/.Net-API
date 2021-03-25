using System.Text.Json.Serialization;

namespace PMAuth.Exceptions.Models
{
    /// <summary>
    /// Error model that will be returned if something will go wrong after user redirection to social network service 
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }
        
        /// <summary>
        /// Detailed description of an error
        /// </summary>
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
        
        /// <summary>
        /// Error reason
        /// </summary>
        [JsonPropertyName("error_reason")]
        public string ErrorReason { get; set; }
    }
}