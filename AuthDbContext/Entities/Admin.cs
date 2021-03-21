using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.AuthDbContext.Entities
{
    /// <summary>
    /// Admin entity
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}
