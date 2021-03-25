using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.Models.SocialsClient
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
        /// List of socials which is registered by user in some application.
        /// </summary>
        public List<Social> Socials { get; set; }
    }
}
