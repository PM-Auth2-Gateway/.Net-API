namespace PMAuth.AuthDbContext.Entities
{
    /// <summary>
    /// App entity
    /// </summary>
    public class App
    {
        /// <summary>
        /// App id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Application Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Admin Id
        /// </summary>
        public int AdminId { get; set; }
        /// <summary>
        /// Admin entity
        /// </summary>
        public Admin Admin { get; set; }
    }
}
