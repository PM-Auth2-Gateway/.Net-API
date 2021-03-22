using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Models;

namespace PMAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly BackOfficeContext _backOfficeContext;

        public AdminController(BackOfficeContext backOfficeContext)
        {
            _backOfficeContext = backOfficeContext;
        }

        [HttpGet]
        [Route("token")]
        public async Task<ActionResult<string>> GetToken([FromHeader] string authCode)
        {
            //todo Authorize
            var admin = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == "admin");
            if (admin == null)
            {
                admin = new Admin() {Name = "admin"};
                _backOfficeContext.Admins.Add(admin);
                await _backOfficeContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet]
        [Route("applications")]
        public async Task<ActionResult<AppModel[]>> GetApps([FromHeader] string Authentication)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var arrApp = _backOfficeContext.Apps.Where(a => a.AdminId == adminId).Select(s=>new AppModel(s.Id,s.Name)).ToArray();
            return Ok(arrApp);
        }

        [HttpPost]
        [Route("applications")]
        public async Task<ActionResult<AppModel>> PostApp([FromHeader] string Authentication, [FromBody] CreateAppModel createApp)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var app = new App() {Name = createApp.Name, AdminId = adminId};
            var resApp = _backOfficeContext.Apps.Add(app).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }

        [HttpGet]
        [Route("applications/{appId}")]
        public async Task<ActionResult<AppModel>> GetAppInfo([FromHeader] string Authentication, [FromRoute] int appId)
        {
            //todo Authorize
            var admin = "admin";
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp == null)
                return BadRequest();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }
        [HttpDelete]
        [Route("applications/{appId}")]
        public async Task<ActionResult> DeleteApp([FromHeader] string Authentication, [FromRoute] int appId)
        {
            //todo Authorize
            var delApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (delApp != null)
            {
                _backOfficeContext.Remove(delApp);
                await _backOfficeContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Route("applications/{appId}")]
        public async Task<ActionResult<AppModel>> PutApp([FromHeader] string Authentication, [FromRoute] int appId, [FromBody] CreateAppModel createApp)
        {
            //todo Authorize
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp != null)
            {
                resApp.Name = createApp.Name;
                await _backOfficeContext.SaveChangesAsync();
                return Ok(new AppModel(resApp.Id,resApp.Name));
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("socials")]
        public async Task<ActionResult<SocialModelResponsecs[]>> GetSocials([FromHeader] string Authentication)
        {
            //todo Authorize
            var admin = "admin";
            var arrSoc = _backOfficeContext.Socials.Select(s=>new SocialModelResponsecs(s.Id,s.Name)).ToArray();
            return Ok(arrSoc);
        }
        [HttpGet]
        [Route("socials/{socialId}")]
        public async Task<ActionResult<SettingModel>> GetSocialSetting([FromHeader] string Authentication, [FromRoute] int socialId,[FromHeader] int appId)
        {

            //todo Authorize
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp == null)
                return BadRequest();
            var resSocial = _backOfficeContext.Socials.FirstOrDefault(a => a.Id == socialId);
            if (resSocial == null)
                return BadRequest();
            var admin = "admin";
            var set = _backOfficeContext.Settings.FirstOrDefault(s => (s.AppId == appId) && (s.SocialId == socialId));
            return Ok(new SettingModel(set.Id, set.AppId, _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name, set.SocialId, _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name, set.ClientId, set.SecretKey, set.Scope));
        }
        
        [HttpPost]
        [Route("socials/{socialId}")]
        public async Task<ActionResult<SettingModel>> PostSocialSetting([FromRoute] int socialId,[FromHeader] string Authentication,[FromHeader] int appId, [FromBody] SocialCreateModel social)
        {
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp == null)
                return BadRequest();
            var resSocial = _backOfficeContext.Socials.FirstOrDefault(a => a.Id == socialId);
            if (resSocial == null)
                return BadRequest();
            //todo Authorize
            var admin = "admin";
            var setting = new Setting()
            {
                ClientId = social.ClientId,
                SecretKey = social.SecretKey,
                Scope = social.Scope,
                AppId = appId,
                SocialId = socialId,
            };
            var set= _backOfficeContext.Settings.Add(setting).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new SettingModel(set.Id,set.AppId, _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name,set.SocialId, _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name,set.ClientId,set.SecretKey,set.Scope));
        }
    }
}
