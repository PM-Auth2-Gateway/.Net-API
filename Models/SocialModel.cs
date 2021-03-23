using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMAuth.Models
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
        /// Redirect Url 2
        /// </summary>
        [JsonPropertyName("redirect_url2")]
        public int RedirectUrl2 { get; set; }
    }
}
