using Microsoft.EntityFrameworkCore;
using PMAuth.AuthDbContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.AuthDbContext
{
    public class BackOfficeContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Social> Socials { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public BackOfficeContext(DbContextOptions<BackOfficeContext> options) : base(options)
        {

        }
    }
}
