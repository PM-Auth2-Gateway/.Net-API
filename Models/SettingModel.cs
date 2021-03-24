using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.Models
{
    public class SettingModel
    {
        /// <summary>
        /// Setting id (identity)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// App Id
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// App name
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Social Id
        /// </summary>
        public int SocialId { get; set; }
        /// <summary>
        /// Social name
        /// </summary>
        public string SocialName { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }

        public SettingModel(int id, int appId, string appName, int socialId, string socialName, string clientId, string secretKey, string scope)
        {
            Id = id;
            AppId = appId;
            AppName = appName;
            SocialId = socialId;
            SocialName = socialName;
            ClientId = clientId;
            SecretKey = secretKey;
            Scope = scope;
        }
    }
}
