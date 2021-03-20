using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.Models
{
    public class SocialsModel
    {
        public List<Social> Socials { get; set; }
    }
}
