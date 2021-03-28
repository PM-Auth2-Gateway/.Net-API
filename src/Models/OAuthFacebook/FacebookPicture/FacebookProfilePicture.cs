using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMAuth.Models.OAuthFacebook.FacebookPicture
{
    /// <summary>
    /// Facebook user's profile picture
    /// </summary>
    public class FacebookProfilePicture
    {
        /// <summary>
        /// Picture data
        /// </summary>
        [JsonPropertyName("data")]
        public FacebookProfilePictureData Data { get; set; }
    }
}
