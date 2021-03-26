using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Models.RequestModels;
using PMAuth.Services.Abstract;

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
        private readonly IEnumerable<IAccessTokenReceivingService> _tokenReceivingServices;
        private readonly IEnumerable<IProfileManagingService> _profileManagingServices;
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;

#pragma warning disable 1591
        public RedirectController(
            IUserProfileReceivingServiceContext userProfileReceivingServiceContext,
            IEnumerable<IAccessTokenReceivingService> tokenReceivingServices,
            IEnumerable<IProfileManagingService> profileManagingServices,
            IMemoryCache memoryCache,
            BackOfficeContext context)
        {
            _userProfileReceivingServiceContext = userProfileReceivingServiceContext;
            _tokenReceivingServices = tokenReceivingServices;
            _profileManagingServices = profileManagingServices;
            _context = context;
            _memoryCache = memoryCache;
        }
#pragma warning restore 1591

        /// <summary>
        /// Handle redirect from Google
        /// </summary>
        /// <param name="error"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        [HttpGet("auth/google")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public IActionResult ReceiveAuthorizationCodeGoogle(
            [FromQuery] RedirectionErrorModelGoogle error,
            [FromQuery] AuthorizationCodeModel authorizationCode)
        {
            if (authorizationCode?.SessionId == null)
            {
                return BadRequest(ErrorModel.AuthError("Session Id is missing."));
            }

            if (error.Error != null || error.ErrorDescription != null)
            {
                return BadRequest(ErrorModel.AuthError($"{error.Error} {error.ErrorDescription}"));
            }
            
            string socialServiceName = "google";
            _userProfileReceivingServiceContext.SetStrategies(
                _tokenReceivingServices.First(s => s.SocialServiceName == socialServiceName),
                _profileManagingServices.First(s => s.SocialServiceName == socialServiceName));

            return ContinueFlow(authorizationCode);
        }

        /// <summary>
        /// Handle redirect from Facebook
        /// </summary>
        /// <param name="error"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        [HttpGet("auth/facebook")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public IActionResult ReceiveAuthorizationCodeFacebook(
            [FromQuery] RedirectionErrorModelFacebook error,
            [FromQuery] AuthorizationCodeModel authorizationCode)
        {
            if (authorizationCode?.SessionId == null)
            {
                return BadRequest(ErrorModel.AuthError("Session Id is missing."));
            }
            
            if (error.Error != null || error.ErrorDescription != null)
            {
                return BadRequest(ErrorModel.AuthError($"{error.Error} {error.ErrorDescription}"));
            }

            string socialServiceName = "facebook";
            _userProfileReceivingServiceContext.SetStrategies(
                _tokenReceivingServices.First(s => s.SocialServiceName == socialServiceName),
                _profileManagingServices.First(s => s.SocialServiceName == socialServiceName));

            _memoryCache.TryGetValue(authorizationCode.SessionId, out CacheModel cache);
            string scope =_context.Settings.FirstOrDefault(x => x.AppId == cache.AppId && x.SocialId == cache.SocialId).Scope;
            authorizationCode.Scope = scope;

            return ContinueFlow(authorizationCode);
        }

        private IActionResult ContinueFlow(AuthorizationCodeModel authorizationCode)
        {
            CacheModel session = _memoryCache.Get<CacheModel>(authorizationCode.SessionId);
            if (session == null)
            {
                return BadRequest(ErrorModel.AuthError("Time for authorization has expired"));
            }
            string device = session.Device.ToLower().Trim();
            session.UserStartedAuthorization = true;

            try
            {
                _userProfileReceivingServiceContext.Execute(session.AppId, authorizationCode);
            }
            catch (AuthorizationCodeExchangeException)
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
