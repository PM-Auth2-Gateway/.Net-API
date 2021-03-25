using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Models.RequestModels;
using PMAuth.Services.Abstract;
using PMAuth.Services.GoogleOAuth;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Controller for handling redirect from social network services
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : ControllerBase
    {
        private readonly IUserProfileReceivingServiceContext _userProfileReceivingServiceContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GoogleAccessTokenReceivingService> _logger;

#pragma warning disable 1591
        public RedirectController(
            IUserProfileReceivingServiceContext userProfileReceivingServiceContext,
            IHttpClientFactory httpClientFactory,
            BackOfficeContext context,
            IMemoryCache memoryCache,
            ILogger<GoogleAccessTokenReceivingService> logger)
        {
            _userProfileReceivingServiceContext = userProfileReceivingServiceContext;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
#pragma warning restore 1591

        /// <summary>
        /// Handle redirect from Google
        /// </summary>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        [HttpGet("auth/google")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public IActionResult ReceiveAuthorizationCodeGoogle(
            string name,
            [FromQuery] RedirectionErrorModelGoogle error, 
            [FromQuery] AuthorizationCodeModel authorizationCode)
        {
            if (error.Error != null || error.ErrorDescription != null)
            {
                return BadRequest(authorizationCode.SessionId);
            }

            _userProfileReceivingServiceContext.SetStrategies(
                new GoogleAccessTokenReceivingService(_httpClientFactory, _context, _memoryCache, _logger),
                new GoogleProfileManager(_memoryCache));
            
            return ContinueFlow(authorizationCode);
        }
        
        /// <summary>
        /// Handle redirect from Facebook
        /// </summary>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        [HttpGet("auth/facebook")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public IActionResult ReceiveAuthorizationCodeFacebook(
            string name,
            [FromQuery] RedirectionErrorModelFacebook error,
            [FromQuery] AuthorizationCodeModel authorizationCode)
        {
            throw new NotImplementedException();
            /*if (//check for error)
            {
                return BadRequest(authorizationCode.SessionId);
            }
        
            //set strategies
            _userProfileReceivingServiceContext.SetStrategies(
                new GoogleAccessTokenReceivingService(_httpClientFactory, _context),
                new GoogleProfileManager(_memoryCache));

            return ContinueFlow(authorizationCode);*/
        }
        
        private IActionResult ContinueFlow(AuthorizationCodeModel authorizationCode)
        {
            CacheModel session = _memoryCache.Get<CacheModel>(authorizationCode.SessionId);
            if (session == null)
            {
                return BadRequest("Authorization time has expired."); 
            }
            string device = session.Device.ToLower().Trim();
            
            try
            {
                _userProfileReceivingServiceContext.Execute(session.AppId, authorizationCode);
            }
            catch (AuthorizationCodeExchangeException exception)
            {
                
            }
            
            if (device.Equals("browser"))
            {
                return new ContentResult 
                {
                    ContentType = "text/html",
                    Content = "<script>window.close()</script>"
                };
            }
            else
            {
                return Redirect(device);
            }
        }
    }
}