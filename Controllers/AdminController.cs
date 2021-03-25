using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Models;
using PMAuth.Services.AuthAdmin;

namespace PMAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly BackOfficeContext _backOfficeContext;
        private readonly AuthService _authService;
        private readonly RefreshTokenService _refreshTokenService;

        public AdminController(BackOfficeContext backOfficeContext,AuthService authService,RefreshTokenService refreshTokenService)
        {
            _backOfficeContext = backOfficeContext;
            _authService = authService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpGet("token")]
        public async Task<ActionResult<AuthModel>> GetToken([FromHeader] string sessionId)
        {
            //todo Authorize
            var identity = _authService.GetIdentity();
            if (identity == null)
            {
                return BadRequest();
            }
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                notBefore: now,
                issuer: AuthOptions.ISSUER,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.SaveRefreshToken(identity.Name, refreshToken);
            return new JsonResult(new AuthModel(identity.Name, encodedJwt, refreshToken));
        }
        [HttpPost("refreshToken")]
        public ActionResult<AuthModel> Refresh(string token, string refreshToken)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = _refreshTokenService.GetRefreshToken(username); //retrieve the refresh token from a data store
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _authService.GenerateToken(principal.Claims);
            var newRefreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.DeleteRefreshToken(username, refreshToken);
            _refreshTokenService.SaveRefreshToken(username, newRefreshToken);

            return new JsonResult(new AuthModel(username,newJwtToken,newRefreshToken));
        }

        [HttpGet]
        [Route("applications")]
        [Authorize]
        public async Task<ActionResult<AppModel[]>> GetApps()
        {
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var arrApp = _backOfficeContext.Apps.Where(a => a.AdminId == adminId).Select(s=>new AppModel(s.Id,s.Name)).ToArray();
            return Ok(arrApp);
        }

        [HttpPost]
        [Route("applications")]
        [Authorize]
        public async Task<ActionResult<AppModel>> PostApp([FromBody] CreateAppModel createApp)
        {
            var admin = "admin";
            var adminId = _backOfficeContext.Admins.First(a => a.Name == admin).Id;
            var app = new App() {Name = createApp.Name, AdminId = adminId};
            var resApp = _backOfficeContext.Apps.Add(app).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }

        [HttpGet]
        [Route("applications/{appId}")]
        [Authorize]
        public async Task<ActionResult<AppModel>> GetAppInfo([FromRoute] int appId)
        {
            var admin = "admin";
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp == null)
                return BadRequest();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }
        [HttpDelete]
        [Route("applications/{appId}")]
        [Authorize]
        public async Task<ActionResult> DeleteApp([FromRoute] int appId)
        {
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
        [Authorize]
        public async Task<ActionResult<AppModel>> PutApp([FromRoute] int appId, [FromBody] CreateAppModel createApp)
        {
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
        [Authorize]
        public async Task<ActionResult<SocialModelResponsecs[]>> GetSocials()
        {
            var admin = "admin";
            var arrSoc = _backOfficeContext.Socials.Select(s=>new SocialModelResponsecs(s.Id,s.Name)).ToArray();
            return Ok(arrSoc);
        }
        [HttpGet]
        [Route("socials/{socialId}")]
        [Authorize]
        public async Task<ActionResult<SettingModel>> GetSocialSetting([FromRoute] int socialId,[FromHeader] int appId)
        {
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
        [Authorize]
        public async Task<ActionResult<SettingModel>> PostSocialSetting([FromRoute] int socialId,[FromHeader] int appId, [FromBody] SocialCreateModel social)
        {
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => a.Id == appId);
            if (resApp == null)
                return BadRequest();
            var resSocial = _backOfficeContext.Socials.FirstOrDefault(a => a.Id == socialId);
            if (resSocial == null)
                return BadRequest();
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
