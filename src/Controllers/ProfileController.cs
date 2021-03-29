using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.RequestModels;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Get user profile by session id
    /// </summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "RegisteredAppAuthentication")]
    public class ProfileController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ProfileController> _logger;

#pragma warning disable 1591
        public ProfileController(IMemoryCache memoryCache, ILogger<ProfileController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
#pragma warning restore 1591 
        
        /// <summary>
        /// Get user profile. Max time of execution for this endpoint is 10 secs
        /// </summary>
        /// <param name="appId">Application id (this id admin receives in the backoffice)</param>
        /// <param name="sessionIdModel">Model which contains session ID</param>
        /// <returns>UserProfile or ErrorModel if profile wasn't found</returns>
        [HttpPost("info")]
        [ProducesResponseType(typeof(UserProfile), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetUserProfileAsync(
            [FromHeader(Name = "App_id")] int appId, 
            [FromBody] SessionIdModel sessionIdModel)
        {
            if (sessionIdModel == null || string.IsNullOrWhiteSpace(sessionIdModel.SessionId))
            {
                _logger.LogError("Request body doesn't contain session id or it is empty");
                return BadRequest(ErrorModel.SessionIdError);
            }

            bool isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out CacheModel sessionInfo);
            if (isSuccess == false || sessionInfo == null)
            {
                _logger.LogError("Unable to find session id in memory cache." +
                                 "Authorization timeout has expired");
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
                _logger.LogError("Unable to find user's profile in memory cache." +
                                 "Error occured during the authorization process");
                return BadRequest(ErrorModel.AuthorizationError("Error occured during the authorization process. " +
                                                       "Unable to receive user's profile for some reasons"));
            }

            return Ok(_memoryCache.Get<CacheModel>(sessionIdModel.SessionId).UserProfile);
        }
    }
}
