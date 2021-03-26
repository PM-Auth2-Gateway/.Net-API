using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.RequestModels;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Get user profile by session id
    /// </summary>
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

#pragma warning disable 1591
        public ProfileController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
#pragma warning restore 1591 
        /// <summary>
        /// Get user profile. Max time of execution for this endpoint is 10 secs
        /// </summary>
        /// <param name="appId">Application id (this id admin receives in the backoffice)</param>
        /// <param name="sessionIdModel">Model which contains session ID</param>
        /// <returns>UserProfile or ErrorModel if profile wasn't found</returns>
        [HttpPost("info")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetUserProfileAsync(
            [FromHeader(Name = "App_id")] int appId, 
            [FromBody] SessionIdModel sessionIdModel)
        {
            //todo add app_id check
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
                return BadRequest(ErrorModel.AuthAborted);
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
                return BadRequest(ErrorModel.AuthError("Error occured during the authorization process. " +
                                                       "Unable to receive user's profile for some reasons"));
            }

            return Ok(_memoryCache.Get<CacheModel>(sessionIdModel.SessionId).UserProfile);
        }
    }
}