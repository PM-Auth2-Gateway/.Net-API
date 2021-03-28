using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMAuth.Models.OAuthFacebook.FacebookPicture
{
    /// <summary>
    /// Facebook user's profile picture data
    /// </summary>
    public class FacebookProfilePictureData
    {
        /// <summary>
        /// Picture height
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Picture is silhouette
        /// </summary>
        [JsonPropertyName("is_silhouette")]
        public bool IsSilhouette { get; set; }

        /// <summary>
        /// Picture url
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Picture width
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}
