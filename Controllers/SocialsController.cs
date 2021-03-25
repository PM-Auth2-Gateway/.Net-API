using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.SocialsClient;

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
        private readonly IMemoryCache cache;

        /// <summary>
        /// SocialsController constructor
        /// </summary>
        /// <param name="context">BackOfficeContext</param>
        /// <param name="cache"></param>
        public SocialsController(BackOfficeContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
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
            var socialIds = context.Settings.Where(x => x.AppId == App_id).Select(x => x.SocialId).Distinct();

            var socials = context.Socials.Where(x => socialIds.Contains(x.Id)).ToList();

            if (socials.Count == 0)
            {
                return BadRequest();
            }

            return new SocialsModel() 
            { 
                Socials = socials
            };

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

            if (setting == null)
            {
                return BadRequest();
            }

            Social social = context.Socials.FirstOrDefault(x => x.Id == setting.SocialId);

            if (social == null)
            {
                return BadRequest();
            }
            
            string socialName = context.Socials.FirstOrDefault(x => x.Id == socialModel.SocialId).Name.ToLower();
            string redirectUri = $"{Request.Scheme}://{Request.Host}/auth/{socialName}";

            var sessionId = Guid.NewGuid().ToString();
            cache.Set(sessionId, new CacheModel
            {
                SocialId = socialModel.SocialId,
                Device = socialModel.Device,
                AppId = App_id,
                RedirectUri = redirectUri
            },
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            });
            
            return new SocialLinkModel
            {
                AuthUri = social.AuthUri,
                RedirectUri = redirectUri,
                ResponseType = "code",
                ClientId = setting.ClientId,
                Scope = setting.Scope,
                State = sessionId
            };
        }
    }
}
