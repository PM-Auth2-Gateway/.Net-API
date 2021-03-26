using Microsoft.EntityFrameworkCore;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.AuthDbContext
{
    /// <summary>
    /// Data Base Context that operate tables in ORM system
    /// </summary>
    public class BackOfficeContext : DbContext
    {
        /// <summary>
        /// Data Base table - Admins
        /// </summary>
        public DbSet<Admin> Admins { get; set; }
        /// <summary>
        /// Data Base table - Apps
        /// </summary>
        public DbSet<App> Apps { get; set; }
        /// <summary>
        /// Data Base table - Socials
        /// </summary>
        public DbSet<Social> Socials { get; set; }
        /// <summary>
        /// Data Base table - Settings
        /// </summary>
        public DbSet<Setting> Settings { get; set; }
        /// <summary>
        /// Data Base table - RefreshTokens
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// BackOfficeContext constructor
        /// </summary>
        /// <param name="options"></param>
        public BackOfficeContext(DbContextOptions<BackOfficeContext> options) : base(options)
        {

        }
    }
}
