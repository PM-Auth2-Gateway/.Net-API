using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.AuthDbContext.Entities
{
    public class App
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
