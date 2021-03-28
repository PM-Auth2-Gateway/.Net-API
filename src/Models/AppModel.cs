using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    /// <summary>
    /// Model application with information
    /// </summary>
    public class AppModel
    {
        /// <summary>
        /// Constructor
        /// </summary>

        public AppModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
        /// <summary>
        /// Application Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }
        /// <summary>
        /// Name application
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }
    }
}
