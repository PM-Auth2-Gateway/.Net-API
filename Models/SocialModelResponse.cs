using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace PMAuth.Models
{
    /// <summary>
    /// Model of information about social 
    /// </summary>
    public class SocialModelResponse
    {
        /// <summary>
        /// Id social
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }
        /// <summary>
        /// Name social
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }
        /// <summary>
        /// True if this social have setting for application
        /// </summary>
        [JsonPropertyName("is_setting")]
        public bool IsSetting { get; }
        /// <summary>
        /// True if this social is active in application
        /// </summary>
        [JsonPropertyName("is_active")]
        public bool IsActive { get; }
        /// <summary>
        /// Path where stored a logo
        /// </summary>
        [JsonPropertyName("logo_path")]
        public string LogoPath { get;}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isSetting"></param>
        public SocialModelResponse(int id, string name, bool isSetting, bool isActive, string logoPath)
        {
            Id = id;
            this.Name = name;
            IsSetting = isSetting;
            IsActive = isActive;
            LogoPath = logoPath;
        }
    }
}
