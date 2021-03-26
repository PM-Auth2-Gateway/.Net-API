using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
    public class RedirectController : ControllerBase
    {
        private readonly IUserProfileReceivingServiceContext _userProfileReceivingServiceContext;
        private readonly IEnumerable<IAccessTokenReceivingService> _tokenReceivingServices;
        private readonly IEnumerable<IProfileManagingService> _profileManagingServices;
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RedirectController> _logger;

#pragma warning disable 1591
        public RedirectController(
            IUserProfileReceivingServiceContext userProfileReceivingServiceContext,
            IEnumerable<IAccessTokenReceivingService> tokenReceivingServices,
            IEnumerable<IProfileManagingService> profileManagingServices,
            IMemoryCache memoryCache,
            BackOfficeContext context, 
            ILogger<RedirectController> logger)
        {
            _userProfileReceivingServiceContext = userProfileReceivingServiceContext;
            _tokenReceivingServices = tokenReceivingServices;
            _profileManagingServices = profileManagingServices;
            _context = context;
            _logger = logger;
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
                _logger.LogError("Received redirect request from Google without session id (state)");
                return BadRequest(ErrorModel.AuthError("Session Id is missing."));
            }

            if (error.Error != null || error.ErrorDescription != null)
            {
                _logger.LogError(@$"Received redirect request from Google with error query params: {error.Error}  |  {error.ErrorDescription}");
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
                _logger.LogError("Received redirect request from Facebook without session id (state)");
                return BadRequest(ErrorModel.AuthError("Session Id is missing."));
            }
            
            if (error.Error != null || error.ErrorDescription != null)
            {
                _logger.LogError(@$"Received redirect request from Facebook with error query params: {error.Error}  |  {error.ErrorDescription}");
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
                _logger.LogError("Authorization time has expired");
                return BadRequest(ErrorModel.AuthError("Time for authorization has expired"));
            }
            string device = session.Device.ToLower().Trim();
            session.UserStartedAuthorization = true;

            try
            {
                _userProfileReceivingServiceContext.Execute(session.AppId, authorizationCode);
            }
            catch (AuthorizationCodeExchangeException exception)
            {
                _logger.LogError("Error occured during authorization code exchange or process of receiving user profile from social service\n " +
                                 $"Error: {exception.Description?.Error}\n ErrorDescription: {exception.Description?.ErrorDescription}");
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
