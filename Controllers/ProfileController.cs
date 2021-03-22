using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions;
using PMAuth.Exceptions.Models;
using PMAuth.Models.OAuthUniversal;
using PMAuth.Services.Abstract;
using PMAuth.Services.GoogleOAuth;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Get user profile by authorization code 
    /// </summary>
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileReceivingServiceContext _userProfileReceivingServiceContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BackOfficeContext _context;

#pragma warning disable 1591
        public ProfileController(
            IUserProfileReceivingServiceContext userProfileReceivingServiceContext,
            IHttpClientFactory httpClientFactory,
            BackOfficeContext context)
        {
            _userProfileReceivingServiceContext = userProfileReceivingServiceContext;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }
#pragma warning restore 1591

       /// <summary>
       /// Get user profile
       /// </summary>
       /// <param name="appId">Application id (this id admin receives in the backoffice)</param>
       /// <param name="authCodeModel">Authorization code model </param>
       /// <returns>UserProfile or AuthorizationCodeExchangeExceptionModel</returns>
        [HttpPost("info")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(AuthorizationCodeExchangeExceptionModel), 400)]
        public async Task<IActionResult> GetUserProfileAsync(
            [FromHeader(Name = "X-APP_ID")] int appId, 
            [FromBody] AuthorizationCodeModel authCodeModel)
        {
            if (authCodeModel.SocialId == _context.Socials.FirstOrDefault(s => s.Name.Equals("Google"))?.Id) // google
            {
                _userProfileReceivingServiceContext.SetStrategies(
                    new GoogleAccessTokenReceivingService(_httpClientFactory, _context),
                    new GoogleProfileManager());
            }
            else if (authCodeModel.SocialId == _context.Socials.FirstOrDefault(s => s.Name.Equals("Facebook"))?.Id) // facebook
            {
                //maybe it should be moved inside receivingServiceContext
                
               /* _userProfileReceivingServiceContext.SetStrategies(
                    new FacebookAccessTokenReceivingService(_httpClientFactory, _context), 
                    new FacebookProfileManager()); */
            }
            else
            {
                AuthorizationCodeExchangeExceptionModel exceptionModel = new AuthorizationCodeExchangeExceptionModel
                {
                    Error = "Unregister social network.",
                    ErrorDescription = "You are trying to use unregister social network. Social network with this ID" +
                                       "doesnt exists."
                };
                return BadRequest(exceptionModel);
            }

            try
            {
                UserProfile userProfile = await _userProfileReceivingServiceContext.Execute(appId, authCodeModel);
                return Ok(userProfile);
            }
            catch (AuthorizationCodeExchangeException exception)
            {
                if (exception.Description != null)
                {
                    return BadRequest(exception.Description);
                }
                else
                {
                    return BadRequest(new AuthorizationCodeExchangeExceptionModel()
                    {
                        ErrorDescription = exception.Message
                    });
                }
            }
        }
    }
}