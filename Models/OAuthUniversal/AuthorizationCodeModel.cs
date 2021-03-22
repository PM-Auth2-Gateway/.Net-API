using System.ComponentModel.DataAnnotations;

namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Authorization code model 
    /// </summary>
    public class AuthorizationCodeModel // this model should be the same for Google and Facebook, I BELIEVE
    {
        /// <summary>
        /// Id of the social network through which authorization is made
        /// </summary>
        [Required]
        public int SocialId { get; set; }
        
        /// <summary>
        /// Authorization code itself
        /// </summary>
        [Required]
        public string AuthorizationCode { get; set; }
        
        /// <summary>
        /// RedirectUri, for now it is hardcoded, so you can type anything here
        /// </summary>
        [Required] 
        public string RedirectUri { get; } = "https%3A%2F%2Fdevelopers.google.com%2Foauthplayground"; 
        // TODO should be received from request. For now it is hardcoded
    }
}