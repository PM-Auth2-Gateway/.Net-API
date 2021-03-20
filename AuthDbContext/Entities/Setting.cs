using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.AuthDbContext.Entities
{
    public class Setting
    {
        public int Id { get; set; }

        public int AppId { get; set; }
        public App App { get; set; }

        public int SocialId { get; set; }
        public Social Social { get; set; }

        public int ClientId { get; set; }
        public string SecretKey { get; set; }
        public string Scope { get; set; }


    }
}
