﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Models;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Return all socials by app_id
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SocialsController : Controller
    {
        private readonly BackOfficeContext context;

        /// <summary>
        /// SocialsController constructor
        /// </summary>
        /// <param name="context">BackOfficeContext</param>
        public SocialsController(BackOfficeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get all socials by app_id
        /// </summary>
        /// <param name="App_id">App_id header</param>
        /// <returns>
        /// <see cref="HttpStatusCode.OK"/> SocialsModel
        /// <see cref="HttpStatusCode.BadRequest"/> for unexisting app_id.
        /// </returns>
        /// <remarks>
        /// Get SocialModel from app_id header
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(SocialsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public ActionResult<SocialsModel> Get([FromHeader] int App_id)
        {
            Setting setting = context.Settings.FirstOrDefault(x => x.AppId == App_id);

            if (setting == null) return BadRequest();

            var socials = context.Socials.Where(x => x.Id == setting.SocialId).ToList();

            if (socials == null) return BadRequest();

            return new SocialsModel() { Socials = socials };

        }
    }
}
