using PMAuth.Models.OAuthUniversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Models
{
    /// <summary>
    /// CasheModel for store info in cache
    /// </summary>
    public class CacheModel
    {
        /// <summary>
        /// Social Id.
        /// </summary>
        public int SocialId { get; set; }

        /// <summary>
        /// Access Token - allow user receiving information about him
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// User Profile where stored user info
        /// </summary>
        public UserProfile UserProfile { get; set; }

        /// <summary>
        /// Device - field which inform what type of device will recieve a redirect
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        public string AppId { get; set; }
    }
}
