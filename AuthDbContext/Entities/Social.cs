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
        /// Auth Uri
        /// </summary>
        public string AuthUri { get; set; }

        /// <summary>
        /// Token Url
        /// </summary>
        public string TokenUrl { get; set; }
    }
}
