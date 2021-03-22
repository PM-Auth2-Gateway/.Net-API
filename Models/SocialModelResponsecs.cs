using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Models
{
    public class SocialModelResponsecs
    {
        public int Id { get; set; }
        public string name { get; set; }

        public SocialModelResponsecs(int id, string name)
        {
            Id = id;
            this.name = name;
        }
    }
}
