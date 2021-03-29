using System.Text.Json.Serialization;

namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Unified model of admin profile
    /// </summary>
    public class AdminProfile:UserProfile
    {
        /// <summary>
        /// Access token what need from authentication
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <inheritdoc />
        public AdminProfile(UserProfile user)
        {
            Id = user?.Id;
            FirstName = user?.FirstName;
            LastName = user?.LastName;
            Email = user?.Email;
            IsVerifiedEmail = user?.IsVerifiedEmail;
            Photo = user?.Photo;
            Locale = user?.Locale;
            AdditionalInformation = user?.AdditionalInformation;
        }
    }
}
