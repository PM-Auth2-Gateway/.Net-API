﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var admin= _backOfficeContext.Admins.FirstOrDefault(a => a.Name == "admin");
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
        public async Task<ActionResult<App[]>> GetApps([FromHeader] string Authentication)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var arrApp = _backOfficeContext.Apps.Where(a => a.AdminId == adminId).ToArray();
            return Ok(arrApp);
        }

        [HttpPost]
        [Route("applications")]
        public async Task<ActionResult<App>> PostApp([FromHeader] string Authentication, [FromHeader] string name)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var app = new App() {Name = name, AdminId = adminId};
            var resApp = _backOfficeContext.Apps.Add(app).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(resApp);
        }

        [HttpGet]
        [Route("applications/{appId}")]
        public async Task<ActionResult<App>> GetAppInfo([FromHeader] string Authentication, [FromRoute] int appId)
        {
            //todo Authorize
            var admin = "admin";
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            return Ok(resApp);
        }
        [HttpDelete]
        [Route("applications/{appId}")]
        public async Task<ActionResult<App>> DeleteApp([FromHeader] string Authentication, [FromRoute] int appId)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var delApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (delApp != null)
            {
                var resApp = _backOfficeContext.Remove(delApp).Entity;
                await _backOfficeContext.SaveChangesAsync();
                return Ok(resApp);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Route("applications/{appId}")]
        public async Task<ActionResult<App>> PutApp([FromHeader] string Authentication, [FromRoute] int appId, [FromHeader] string name)
        {
            //todo Authorize
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp != null)
            {
                resApp.Name = name;
                await _backOfficeContext.SaveChangesAsync();
                return Ok(resApp);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("socials")]
        public async Task<ActionResult<Social[]>> GetSocials([FromHeader] string Authentication)
        {
            //todo Authorize
            var admin = "admin";
            var arrSoc = _backOfficeContext.Socials.ToArray();
            return Ok(arrSoc);
        }
        [HttpGet]
        [Route("socials/{socialId}")]
        public async Task<ActionResult<Setting>> GetSocialSetting([FromHeader] string Authentication, [FromRoute] int socialId,[FromHeader] int appId)
        {
            //todo Authorize
            var admin = "admin";
            var setting = _backOfficeContext.Settings.FirstOrDefault(s => (s.AppId == appId) && (s.SocialId == socialId));

            return Ok(setting);
        }
        
        [HttpPost]
        [Route("socials")]
        public async Task<ActionResult<Setting>> PostSocialSetting([FromHeader] int socialId,[FromHeader] string Authentication,[FromHeader] int appId, [FromBody] SocialCreateModel social)
        {
            //todo Authorize
            var admin = "admin";
            var setting = new Setting()
            {
                ClientId = social.ClientId,
                SecretKey = social.SecretKey,
                Scope = social.Scope,
                AppId = appId,
                SocialId = socialId
            };
            _backOfficeContext.Settings.Add(setting);
            await _backOfficeContext.SaveChangesAsync();
            return Ok(setting);
        }
    }
}
