using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Exceptions.Models;
using PMAuth.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.RequestModels;
using PMAuth.Services.AuthAdmin;

namespace PMAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AdminController> _logger;
        private readonly BackOfficeContext _backOfficeContext;
        private readonly AuthService _authService;
        private readonly RefreshTokenService _refreshTokenService;

        public AdminController(
            BackOfficeContext backOfficeContext,
            AuthService authService,
            RefreshTokenService refreshTokenService,
            IMemoryCache memoryCache,
            ILogger<AdminController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _backOfficeContext = backOfficeContext;
            _authService = authService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("tokenAndProfile")]
        public async Task<ActionResult<AdminProfile>> GetTokenAndProfile(
        [FromHeader(Name = "App_id")] int appId,
        [FromBody] SessionIdModel sessionIdModel)
        {
            _logger.LogInformation($"SessionId: {sessionIdModel?.SessionId}");
            if (sessionIdModel == null || string.IsNullOrWhiteSpace(sessionIdModel.SessionId))
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Invalid session id",
                    ErrorDescription = "There is no profile related to provided session id"
                });
            }

            bool isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out CacheModel sessionInfo);
            if (isSuccess == false || sessionInfo == null)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Invalid session id",
                    ErrorDescription = "There is no profile related to provided session id"
                });
            }
            isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out sessionInfo);
            if (isSuccess && sessionInfo?.UserProfile == null)
            {
                int requestCounter = 0;
                while (requestCounter < 50)
                {
                    _logger.LogInformation($"AccessToken: {sessionInfo?.AccessToken}");
                    _logger.LogInformation($"Email: {sessionInfo?.UserProfile?.Email}");
                    isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out sessionInfo);
                    if (isSuccess && sessionInfo?.UserProfile != null)
                    {
                        break;
                    }

                    await Task.Delay(200);
                    requestCounter++;
                }
            }


            if (sessionInfo?.UserProfile == null)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Social service servers are currently unavailable",
                    ErrorDescription = "There is no profile related to provided session id"
                });
            }

            var admin = (AdminProfile)(_memoryCache.Get<CacheModel>(sessionIdModel.SessionId).UserProfile);
            //todo Authorize
            if (_backOfficeContext.Admins.FirstOrDefault(a => a.Name == admin.Id) == null)
                _backOfficeContext.Admins.Add(new Admin() {Name = admin.Id});
            var identity = _authService.GetIdentity(admin.Id);
            if (identity == null)
            {
                return BadRequest();
            }
            var jwt = _authService.GenerateToken(identity.Claims);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.SaveRefreshToken(identity.Name, refreshToken);
            Response.Cookies.Append("X-Refresh-Token", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true
                });
            admin.Token = encodedJwt;
            return new JsonResult(admin);
        }

        [HttpPost("refreshToken")]
        public ActionResult<AuthModel> Refresh(string token)
        {
            var refreshToken = Request.Cookies?["X-Refresh-Token"];
            if (refreshToken == null)
                return BadRequest();
            var principal = _authService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = _refreshTokenService.GetRefreshToken(username); //retrieve the refresh token from a data store
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _authService.GenerateToken(principal.Claims);
            var newRefreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.DeleteRefreshToken(username);
            _refreshTokenService.SaveRefreshToken(username, newRefreshToken);
            Response.Cookies.Append("X-Refresh-Token",refreshToken,
                new CookieOptions
                {
                    HttpOnly = true
                });
            return new JsonResult(new AuthModel(username, new JwtSecurityTokenHandler().WriteToken(newJwtToken)));
        }

        [HttpGet]
        [Route("applications")]
        [Authorize]
        public async Task<ActionResult<AppModel[]>> GetApps()
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var arrApp = _backOfficeContext.Apps.Where(a => a.AdminId == adminId).Select(s=>new AppModel(s.Id,s.Name)).ToArray();
            return Ok(arrApp);
        }

        [HttpPost]
        [Route("applications")]
        [Authorize]
        public async Task<ActionResult<AppModel>> PostApp([FromBody] CreateAppModel createApp)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var app = new App() {Name = createApp.Name, AdminId = (int)adminId };
            var resApp = _backOfficeContext.Apps.Add(app).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }

        [HttpGet]
        [Route("applications/{appId}")]
        [Authorize]
        public async Task<ActionResult<AppModel>> GetAppInfo([FromRoute] int appId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            if (resApp == null)
                return BadRequest();
            return Ok(new AppModel(resApp.Id,resApp.Name));
        }
        [HttpDelete]
        [Route("applications/{appId}")]
        [Authorize]
        public async Task<ActionResult> DeleteApp([FromRoute] int appId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            if (resApp == null)
                return BadRequest();
            _backOfficeContext.Remove(resApp);
                await _backOfficeContext.SaveChangesAsync();
                return Ok();

        }

        [HttpPut]
        [Route("applications/{appId}")]
        [Authorize]
        public async Task<ActionResult<AppModel>> PutApp([FromRoute] int appId, [FromBody] CreateAppModel createApp)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            if (resApp == null)
                return BadRequest();
            resApp.Name = createApp.Name;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id, resApp.Name));

        }


        [HttpGet]
        [Route("applications/{appId}/socials")]
        [Authorize]
        public async Task<ActionResult<SocialModelResponsecs[]>> GetSocials([FromRoute] int appId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            if (resApp == null)
                return BadRequest();
            var arrSoc = _backOfficeContext.Socials.Select(
                s => new SocialModelResponsecs(s.Id, s.Name, _backOfficeContext.Settings.Any(s => s.SocialId == s.Id))).ToArray();
            return Ok(arrSoc);
        }
        [HttpGet]
        [Route("applications/{appId}/socials/{socialId}")]
        [Authorize]
        public async Task<ActionResult<SettingModel>> GetSocialSetting([FromRoute] int socialId,[FromRoute] int appId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
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
        [Route("applications/{appId}/socials/{socialId}")]
        [Authorize]
        public async Task<ActionResult<SettingModel>> PostSocialSetting([FromRoute] int socialId,[FromRoute] int appId, [FromBody] SocialCreateModel social)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            if (adminId == null)
                return BadRequest();
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId)&&(a.AdminId == adminId));
            if (resApp == null)
                return BadRequest();
            var resSocial = _backOfficeContext.Socials.FirstOrDefault(a => a.Id == socialId);
            if (resSocial == null)
                return BadRequest();
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
