using Microsoft.EntityFrameworkCore;
using PMAuth.AuthDbContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.AuthDbContext
{
    /// <summary>
    /// BackOfficeContext
    /// </summary>
    public class BackOfficeContext : DbContext
    {
        /// <summary>
        /// Admins DbSet
        /// </summary>
        public DbSet<Admin> Admins { get; set; }
        /// <summary>
        /// Apps DbSet
        /// </summary>
        public DbSet<App> Apps { get; set; }
        /// <summary>
        /// Socials DbSet
        /// </summary>
        public DbSet<Social> Socials { get; set; }
        /// <summary>
        /// Settings DbSet
        /// </summary>
        public DbSet<Setting> Settings { get; set; }
        /// <summary>
        /// BackOfficeContext constructor
        /// </summary>
        /// <param name="options"></param>
        public BackOfficeContext(DbContextOptions<BackOfficeContext> options) : base(options)
        {

        }
    }
}
