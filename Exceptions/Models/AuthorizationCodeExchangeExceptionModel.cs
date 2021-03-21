using System.Text.Json.Serialization;

namespace PMAuth.Exceptions.Models
{
    public class AuthorizationCodeExchangeExceptionModel
    {
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}