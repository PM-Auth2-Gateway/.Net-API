using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Models.RequestModels;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Get user profile by authorization code 
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
        /// Get user profile
        /// </summary>
        /// <param name="appId">Application id (this id admin receives in the backoffice)</param>
        /// <param name="sessionIdModel">Model which contains session ID</param>
        /// <returns>UserProfile or ErrorModel</returns>
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
                return BadRequest(new ErrorModel
                {
                    Error = "Invalid session id",
                    ErrorDescription = "There is no profile related to provided session id"
                });
            }

            bool isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out TempDummyMc sessionInfo);
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
                while (requestCounter < 20)
                {
                    isSuccess = _memoryCache.TryGetValue(sessionIdModel.SessionId, out sessionInfo);
                    if (isSuccess && sessionInfo?.UserProfile != null)
                    {
                        break;
                    }

                    await Task.Delay(500);
                    requestCounter++;
                }
            }
            
            if (sessionInfo?.UserProfile == null)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Invalid session id",
                    ErrorDescription = "There is no profile related to provided session id"
                });
            }

            return Ok(_memoryCache.Get<TempDummyMc>(sessionIdModel.SessionId).UserProfile);
        }
    }
}