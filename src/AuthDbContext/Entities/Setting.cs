namespace PMAuth.AuthDbContext.Entities
{
    /// <summary>
    /// Setting entity
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Setting id (identity)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// App entity
        /// </summary>
        public App App { get; set; }

        /// <summary>
        /// Social Id
        /// </summary>
        public int SocialId { get; set; }
        /// <summary>
        /// Social entity
        /// </summary>
        public Social Social { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Secret Key - secret field setting in application
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// Scope - bunch of accessed information by user.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Is a social disabled or enabled 
        /// </summary>
        public bool IsActive { get; set; }

    }
}
