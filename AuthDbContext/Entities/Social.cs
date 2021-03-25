namespace PMAuth.AuthDbContext.Entities
{
    /// <summary>
    /// Social entity
    /// </summary>
    public class Social
    {
        /// <summary>
        /// Social id.
        /// </summary>
        /// <remarks>Primary key.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Social name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Auth Uri - base link of some service which offer authentication.
        /// </summary>
        public string AuthUri { get; set; }

        /// <summary>
        /// Token Url - adress which operate with tokens
        /// </summary>
        public string TokenUrl { get; set; }

        /// <summary>
        /// Path where stored a logo
        /// </summary>
        public string LogoPath { get; set; }
    }
}
