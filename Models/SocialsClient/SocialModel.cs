using System.Text.Json.Serialization;

namespace PMAuth.Models.SocialsClient
{
    /// <summary>
    /// Requested social model
    /// </summary>
    public class SocialModel
    {
        /// <summary>
        /// Social Id.
        /// </summary>
        [JsonPropertyName("social_id")]
        public int SocialId { get; set; }

        /// <summary>
        /// Device - field which inform what type of device will recieve a redirect
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }
    }
}
