using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.Models
{
    /// <summary>
    /// A social model.
    /// </summary>
    /// <remarks>
    /// Model has list of socials.
    /// </remarks>
    public class SocialsModel
    {
        /// <summary>
        /// List of socials.
        /// </summary>
        public List<Social> Socials { get; set; }
    }
}
