using System;
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
    /// Controller that operate socials.
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
        public ActionResult<SocialsModel> GetAllSocials([FromHeader] int App_id)
        {
            var socialIds = context.Settings.Where(x => x.AppId == App_id).Select(x => x.SocialId).Distinct().ToList();

            if (socialIds == null) return BadRequest();

            List<Social> socials = new List<Social>();
            foreach(int socialId in socialIds)
            {
                socials.Add(context.Socials.FirstOrDefault(social => social.Id == socialId));
            }
            
            if (socials.Count == 0) return BadRequest();

            return new SocialsModel() { Socials = socials };

        }


        /// <summary>
        /// Get parameters for social auth link.
        /// </summary>
        /// <param name="App_id">App_id header</param>
        /// <param name="socialModel">Social Model</param>
        /// <returns>
        /// <see cref="HttpStatusCode.OK"/> SocialLinkModel
        /// <see cref="HttpStatusCode.BadRequest"/> for unexisting app_id or social_id.
        /// </returns>
        /// <remarks>
        /// Get SocialLinkModel from app_id header and social_id value.
        /// </remarks>
        [HttpPost("auth-link")]
        [ProducesResponseType(typeof(SocialLinkModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public ActionResult<SocialLinkModel> GetLinkParameters([FromHeader] int App_id, [FromBody] SocialModel socialModel)
        {
            Setting setting = context.Settings.FirstOrDefault(x => x.AppId == App_id && x.SocialId == socialModel.SocialId);

            if (setting == null) return BadRequest();

            Social social = context.Socials.FirstOrDefault(x => x.Id == setting.SocialId);

            if (social == null) return BadRequest();

            return new SocialLinkModel()
            {
                AuthUri = social.AuthUri,
                RedirectUri = "redirect_uri",
                ResponseType = "code",
                ClientId = setting.ClientId,
                Scope = setting.Scope,
                State = "session_id"
            };
        }
    }
}
