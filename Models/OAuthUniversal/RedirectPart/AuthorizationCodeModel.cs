using Microsoft.AspNetCore.Mvc;

namespace PMAuth.Models.OAuthUniversal.RedirectPart
{
    /// <summary>
    /// Authorization code model 
    /// </summary>
    public class AuthorizationCodeModel // this model should be the same for Google and Facebook, I BELIEVE
    {
        /// <summary>
        /// Authorization code
        /// </summary>
        [FromQuery(Name = "code")]
        public string AuthorizationCode { get; set; }
        
        /// <summary>
        /// Scope
        /// </summary>
        [FromQuery(Name = "scope")]
        public string Scope { get; set; }
        
        /// <summary>
        /// Session ID
        /// </summary>
        [FromQuery(Name = "state")]
        public string SessionId { get; set; }
    }
}