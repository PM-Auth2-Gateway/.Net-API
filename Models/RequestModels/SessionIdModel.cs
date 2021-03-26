using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMAuth.Models.RequestModels
{
    public class SessionIdModel
    {
        [Required]
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
