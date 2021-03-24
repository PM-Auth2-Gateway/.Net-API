using System;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.OAuthUniversal.RedirectPart;
using PMAuth.Services.Abstract;
using PMAuth.Services.FacebookOAuth;
using PMAuth.Services.GoogleOAuth;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Controller for handling redirect from social network services
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Route("[controller]")]
    public class RedirectController : ControllerBase
    {
        private readonly IUserProfileReceivingServiceContext _userProfileReceivingServiceContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BackOfficeContext _context;
        private readonly IMemoryCache _memoryCache;

#pragma warning disable 1591
        public RedirectController(
            IUserProfileReceivingServiceContext userProfileReceivingServiceContext,
            IHttpClientFactory httpClientFactory,
            BackOfficeContext context,
            IMemoryCache memoryCache)
        {
            _userProfileReceivingServiceContext = userProfileReceivingServiceContext;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _memoryCache = memoryCache;
            //todo delete
            _memoryCache.Set("xxxxxxxxxxxxxxxxx", new TempDummyMc
            {
                Device = "browser"
            }, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        [HttpGet("auth/{name}")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public IActionResult ReceiveAuthorizationCode(
            string name,
            [FromQuery] ErrorModel error, 
            [FromQuery] AuthorizationCodeModel authorizationCode)
        {
            if (error.Error != null || error.ErrorDescription != null)
            {
                return BadRequest(authorizationCode.SessionId);
            }
            
            if (name.ToLower().Trim().Equals("google")) // google
            {
                _userProfileReceivingServiceContext.SetStrategies(
                    new GoogleAccessTokenReceivingService(_httpClientFactory, _context),
                    new GoogleProfileManager(_memoryCache));
            }
            else if (name.ToLower().Trim().Equals("facebook")) // facebook
            {
                //maybe it should be moved inside receivingServiceContext

                _userProfileReceivingServiceContext.SetStrategies(
                    new FacebookAccessTokenReceivingService(_httpClientFactory, _context),
                    new FacebookProfileManager(_memoryCache));
            }
            else
            {
                ErrorModel exceptionModel = new ErrorModel
                {
                    Error = "Unregister social network.",
                    ErrorDescription = "You are trying to use unregister social network. Social network with this ID" +
                                       "doesnt exists."
                };
                return BadRequest(exceptionModel);
            }
            
            try
            {
                _userProfileReceivingServiceContext.Execute(1, authorizationCode);
            }
            catch (AuthorizationCodeExchangeException exception)
            {
                
            }

            bool isSuccess = _memoryCache.TryGetValue(authorizationCode.SessionId, out TempDummyMc sessionInfo);
            if (isSuccess == false || string.IsNullOrWhiteSpace(sessionInfo?.Device))
            {
                return BadRequest("Unknown session ID");
            }

            if (sessionInfo.Device.ToLower().Trim().Equals("browser"))
            {
                return new ContentResult 
                {
                    ContentType = "text/html",
                    Content = "<script>window.close()</script>"
                };
            }
            else
            {
                return Redirect(sessionInfo.Device);
            }
        }
    }
}