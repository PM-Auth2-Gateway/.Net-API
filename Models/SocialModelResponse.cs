using System.Text.Json.Serialization;

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
        [JsonPropertyName("isSetting")]
        public bool IsSetting { get; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isSetting"></param>
        public SocialModelResponse(int id, string name, bool isSetting)
        {
            Id = id;
            this.Name = name;
            IsSetting = isSetting;
        }
    }
}
