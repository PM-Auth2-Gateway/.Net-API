namespace PMAuth.Models.OAuthUniversal
{
    /// <summary>
    /// Authorization code model 
    /// </summary>
    public class AuthorizationCodeModel // this model should be the same for Google and Facebook, I BELIEVE
    {
        /// <summary>
        /// Authorization code itself
        /// </summary>
        public string AuthorizationCode { get; set; }
        
        /// <summary>
        /// RedirectUri, for now it is hardcoded, so you can type anything here
        /// </summary>
        public string RedirectUri { get; } = "https%3A%2F%2Fdevelopers.google.com%2Foauthplayground"; // TODO
    }
}