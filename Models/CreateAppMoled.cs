using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMAuth.Models
{
    /// <summary>
    /// Model of information about application will be created
    /// </summary>
    public class CreateAppModel
    {
        /// <summary>
        /// Name application
        /// </summary>
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
    }
}
