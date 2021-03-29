using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using PMAuth.Services.AuthAdmin.Interface;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Admin Api for access to setting application
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AdminController> _logger;
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IAuthServise _authService;
        private readonly IRefreshTokenService _refreshTokenService;

        /// <inheritdoc />
        public AdminController(
            BackOfficeContext backOfficeContext,
            IAuthServise authService,
            IRefreshTokenService refreshTokenService,
            IMemoryCache memoryCache,
            ILogger<AdminController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _backOfficeContext = backOfficeContext;
            _authService = authService;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Get token for testing
        /// </summary>
        /// <returns>Token</returns>
        [HttpPost("testToken")]
        [AllowAnonymous]
        public ActionResult<string> GetTokenTest()
        {
            var identity = _authService.GetIdentity("admin");
            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                claims: identity.Claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );
            var refreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.SaveRefreshToken(identity.Name,refreshToken);
            Response.Cookies.Append("X-Refresh-Token", refreshToken,
                new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.UtcNow.AddDays(7), SameSite = SameSiteMode.None });
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Get admin profile with token access
        /// </summary>
        /// <param name="sessionIdModel">Model which contains session ID</param>
        /// <returns>AdminProfile or ErrorModel if profile wasn't found</returns>
        [HttpPost("tokenAndProfile")]
        [ProducesResponseType(typeof(AdminProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [AllowAnonymous]
        public async Task<ActionResult<AdminProfile>> GetTokenAndProfile(
            [FromBody, Required] SessionIdModel sessionIdModel)
        {
            if (sessionIdModel == null || string.IsNullOrWhiteSpace(sessionIdModel.SessionId))
            {
                return BadRequest(ErrorModel.SessionIdError);
            }

            bool isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out CacheModel sessionInfo);
            if (isSuccess == false || sessionInfo == null)
            {
                return BadRequest(ErrorModel.SessionIdError);
            }

            if (sessionInfo.UserStartedAuthorization == false)
            {
                return BadRequest(ErrorModel.AuthorizationAborted);
            }

            isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out sessionInfo);
            if (isSuccess && sessionInfo?.UserProfile == null)
            {
                int requestCounter = 0;
                while (requestCounter < 50)
                {
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
                return BadRequest(ErrorModel.AuthorizationError("Error occured during the authorization process. " +
                                                                "Unable to receive user's profile for some reasons"));
            }

            var admin = new AdminProfile(_memoryCache.Get<CacheModel>(sessionIdModel.SessionId).UserProfile);

            if (_backOfficeContext.Admins.FirstOrDefault(a => a.Name == admin.Id) == null)
            {
                await _backOfficeContext.Admins.AddAsync(new Admin() {Name = admin.Id});
                await _backOfficeContext.SaveChangesAsync();
            }

            var identity = _authService.GetIdentity(admin.Id);
            var jwt = _authService.GenerateToken(identity.Claims);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.SaveRefreshToken(identity.Name,refreshToken);

            Response.Cookies.Append("X-Refresh-Token", refreshToken,
                new CookieOptions { HttpOnly = true,Secure = true,Expires = DateTime.UtcNow.AddDays(7), SameSite = SameSiteMode.None });
            admin.Token = encodedJwt;
            return new JsonResult(admin);
        }

        /// <summary>
        /// Get new access token 
        /// </summary>
        /// <param name="token">Old access token</param>
        /// <returns>AuthModel or error if refresh token wasn't found</returns>
        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(AuthModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [AllowAnonymous]
        public ActionResult<AuthModel> Refresh([Required, FromHeader] string token)
        {
            var refreshToken = Request.Cookies?["X-Refresh-Token"];
            if (refreshToken == null)
                return BadRequest(ErrorModel.TokenErrorModel("Http request don't have refreshToken"));
            var principal = _authService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            if (!_refreshTokenService.CheckRefreshToken(username,refreshToken))
                return BadRequest(ErrorModel.TokenErrorModel("Server don't have refresh token"));

            var newJwtToken = _authService.GenerateToken(principal.Claims);
            var newRefreshToken = _authService.GenerateRefreshToken();
            _refreshTokenService.DeleteRefreshToken(username,refreshToken);
            _refreshTokenService.SaveRefreshToken(username,newRefreshToken);
            Response.Cookies.Append("X-Refresh-Token", newRefreshToken,
                new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.UtcNow.AddDays(7), SameSite = SameSiteMode.None});
            return new JsonResult(new AuthModel(username, new JwtSecurityTokenHandler().WriteToken(newJwtToken)));
        }

        /// <summary>
        /// Get all applications for logged in admin
        /// </summary>
        /// <returns>AppModel[] or error if refresh admin wasn't found</returns>
        [HttpGet]
        [Route("applications")]
        [ProducesResponseType(typeof(AppModel[]), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public ActionResult<AppModel[]> GetApps()
        {
            string error = _authService.CheckId(User.Identity.Name);
            if (error != null)
            {
                return BadRequest(error);
            }

            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            var arrApp = _backOfficeContext.Apps.Where(a => a.AdminId == adminId)
                .Select(s => new AppModel(s.Id, s.Name)).ToArray();
            return Ok(arrApp);
        }

        /// <summary>
        /// Post new application 
        /// </summary>
        /// <param name="createApp">Information about application</param>
        /// <returns>AppModel or error if admin Id wasn't found</returns>
        [HttpPost]
        [Route("applications")]
        [ProducesResponseType(typeof(AppModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<AppModel>> PostApp([FromBody, Required] CreateAppModel createApp)
        {
            string error = _authService.CheckId(User.Identity.Name);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            // ReSharper disable once PossibleInvalidOperationException
            var app = new App() {Name = createApp.Name, AdminId = (int)adminId};
            var resApp = (await _backOfficeContext.Apps.AddAsync(app)).Entity;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id, resApp.Name));
        }

        /// <summary>
        /// Get information about application 
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <returns>AppModel or error if admin,app Id wasn't found</returns>
        [HttpGet]
        [Route("applications/{appId}")]
        [ProducesResponseType(typeof(AppModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public ActionResult<AppModel> GetAppInfo([FromRoute, Required] int appId)
        {
            string error = _authService.CheckId(User.Identity.Name, appId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            // ReSharper disable once PossibleNullReferenceException
            return Ok(new AppModel(resApp.Id, resApp.Name));
        }

        /// <summary>
        /// Delete application 
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <returns>200 or error if admin,app Id wasn't found</returns>
        [HttpDelete]
        [Route("applications/{appId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult> DeleteApp([FromRoute, Required] int appId)
        {
            string error = _authService.CheckId(User.Identity.Name, appId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            _backOfficeContext.Apps.Remove(resApp!);
            await _backOfficeContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Put information of application 
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <param name="createApp">Object save information about application</param>
        /// <returns>AppModel or error if admin,app id wasn't found</returns>
        [HttpPut]
        [Route("applications/{appId}")]
        [ProducesResponseType(typeof(AppModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<AppModel>> PutApp([FromRoute, Required] int appId,
            [FromBody, Required] CreateAppModel createApp)
        {
            string error = _authService.CheckId(User.Identity.Name, appId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == User.Identity.Name)?.Id;
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            resApp.Name = createApp.Name;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(new AppModel(resApp.Id, resApp.Name));
        }

        /// <summary>
        /// Get list of Socials which have settings for application
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <returns>SocialModelResponse[] or error if admin,app id wasn't found</returns>
        [HttpGet]
        [Route("applications/{appId}/socials")]
        [ProducesResponseType(typeof(SocialModelResponse[]), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<SocialModelResponse[]>> GetSocials([FromRoute, Required] int appId)
        {
            string error = _authService.CheckId(User.Identity.Name, appId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var arrSoc = _backOfficeContext.Socials.Select(s =>
                    new SocialModelResponse(s.Id, s.Name,
                        _backOfficeContext.Settings.Any(set => (s.Id == set.SocialId) && (set.AppId == appId)),
                        _backOfficeContext.Settings.Any(set => (s.Id == set.SocialId) && (set.AppId == appId))
                        && _backOfficeContext.Settings.First(set => (s.Id == set.SocialId) && (set.AppId == appId))
                            .IsActive,
                        s.LogoPath))
                .ToArray();
            return Ok(arrSoc);
        }

        /// <summary>
        /// Get settings of application
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <param name="socialId">Id social</param>
        /// <returns>SettingModel or error if admin,app,social id wasn't found</returns>
        [HttpGet]
        [Route("applications/{appId}/socials/{socialId}")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public ActionResult<SettingModel> GetSocialSetting([FromRoute, Required] int socialId,
            [FromRoute, Required] int appId)
        {
            string error = _authService.CheckId(User.Identity.Name, appId, socialId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var set = _backOfficeContext.Settings.First(s => (s.AppId == appId) && (s.SocialId == socialId));
            return Ok(
                new SettingModel(
                    set.Id,
                    set.AppId,
                    _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name,
                    set.SocialId,
                    _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name,
                    set.ClientId, set.SecretKey, set.Scope, set.IsActive));
        }

        /// <summary>
        /// Post social settings of application
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <param name="socialId">Id social</param>
        /// <param name="social">information of social setting</param>
        /// <returns>SettingModel or error if admin,app,social id wasn't found</returns>
        [HttpPost]
        [Route("applications/{appId}/socials/{socialId}")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<SettingModel>> PostSocialSetting([FromRoute, Required] int socialId,
            [FromRoute, Required] int appId, [FromBody] SocialCreateModel social)
        {
            string error = _authService.CheckId(User.Identity.Name, appId, socialId);
            switch (error)
            {
                case "This setting is not existed":
                {
                    var setting = new Setting()
                    {
                        ClientId = social.ClientId,
                        SecretKey = social.SecretKey,
                        Scope = social.Scope,
                        AppId = appId,
                        SocialId = socialId,
                        IsActive = true
                    };
                    var set = (await _backOfficeContext.Settings.AddAsync(setting)).Entity;
                    await _backOfficeContext.SaveChangesAsync();
                    return Ok(new SettingModel(set.Id, set.AppId,
                        _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name, set.SocialId,
                        _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name, set.ClientId,
                        set.SecretKey,
                        set.Scope, set.IsActive));
                }
                case null:
                    return BadRequest(ErrorModel.IdErrorModel("This setting already exist"));
                default:
                    return BadRequest(ErrorModel.IdErrorModel(error));
            }
        }

        /// <summary>
        /// Put social settings of application
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <param name="socialId">Id social</param>
        /// <param name="social">information of social setting</param>
        /// <returns>SettingModel or error if admin,app,social id wasn't found</returns>
        [HttpPut]
        [Route("applications/{appId}/socials/{socialId}")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<SettingModel>> PutSocialSetting([FromRoute, Required] int socialId,
            [FromRoute, Required] int appId, [FromBody] SocialCreateModel social)
        {
            string error = _authService.CheckId(User.Identity.Name, appId, socialId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var set = _backOfficeContext.Settings.First(s => (s.AppId == appId) && (s.SocialId == socialId));
            set.ClientId = social.ClientId;
            set.SecretKey = social.SecretKey;
            set.Scope = social.Scope;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(
                new SettingModel(
                    set.Id,
                    set.AppId,
                    _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name,
                    set.SocialId,
                    _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name,
                    set.ClientId, set.SecretKey, set.Scope, set.IsActive));
        }

        /// <summary>
        /// Post social active settings of application
        /// </summary>
        /// <param name="appId">Id application</param>
        /// <param name="socialId">Id social</param>
        /// <param name="isActive">information of social activity</param>
        /// <returns>SettingModel or error if admin,app,social id wasn't found</returns>
        [HttpPost]
        [Route("applications/{appId}/socials/{socialId}/{isActive}")]
        [ProducesResponseType(typeof(SettingModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<SettingModel>> PostSocialSettingActive([FromRoute, Required] int socialId,
            [FromRoute, Required] int appId, [FromRoute, Required] bool isActive)
        {
            string error = _authService.CheckId(User.Identity.Name, appId, socialId);
            if (error != null)
            {
                return BadRequest(ErrorModel.IdErrorModel(error));
            }

            var set = _backOfficeContext.Settings.First(s => (s.AppId == appId) && (s.SocialId == socialId));
            set.IsActive = isActive;
            await _backOfficeContext.SaveChangesAsync();
            return Ok(
                new SettingModel(
                    set.Id,
                    set.AppId,
                    _backOfficeContext.Apps.FirstOrDefault(s => s.Id == appId)?.Name,
                    set.SocialId,
                    _backOfficeContext.Socials.FirstOrDefault(s => s.Id == socialId)?.Name,
                    set.ClientId, set.SecretKey, set.Scope, set.IsActive));
        }
    }
}
