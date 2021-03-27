using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    public class SettingModel
    {
        /// <summary>
        /// Setting id (identity)
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// App Id
        /// </summary>
        [JsonPropertyName("app_id")]
        public int AppId { get;  }
        /// <summary>
        /// App name
        /// </summary>
        [JsonPropertyName("app_name")]
        public string AppName { get;  }

        /// <summary>
        /// Social Id
        /// </summary>
        [JsonPropertyName("social_id")]
        public int SocialId { get;  }
        /// <summary>
        /// Social name
        /// </summary>
        [JsonPropertyName("social_name")]
        public string SocialName { get; }

        /// <summary>
        /// Client Id
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get;  }
        /// <summary>
        /// Secret Key
        /// </summary>
        [JsonPropertyName("secret_key")]
        public string SecretKey { get;  }
        /// <summary>
        /// Scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get;  }

        /// <summary>
        /// True if this social is active in application
        /// </summary>
        [JsonPropertyName("is_active")]
        public bool IsActive { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="socialId"></param>
        /// <param name="socialName"></param>
        /// <param name="clientId"></param>
        /// <param name="secretKey"></param>
        /// <param name="scope"></param>
        public SettingModel(int id, int appId, string appName, int socialId, string socialName, string clientId, string secretKey, string scope,bool isActive)
        {
            Id = id;
            AppId = appId;
            AppName = appName;
            SocialId = socialId;
            SocialName = socialName;
            ClientId = clientId;
            SecretKey = secretKey;
            Scope = scope;
            IsActive = isActive;
        }
    }
}
