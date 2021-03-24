using System.Text.Json.Serialization;

namespace PMAuth.Models.RequestModels
{
    public class SessionIdModel
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}