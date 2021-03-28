using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    public class AuthModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        public AuthModel(string name, string token)
        {
            Name = name;
            Token = token;
        }

        /// <summary>
        /// Name admin
        /// </summary>
        [JsonPropertyName("name")]
        public  string Name { get; set; }
        /// <summary>
        /// Jwt Access token
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
