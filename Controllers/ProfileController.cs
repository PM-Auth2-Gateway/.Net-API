using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PMAuth.Controllers
{
    /// <summary>
    /// Get user profile by authorization code 
    /// </summary>
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        public ProfileController()
        {
            
        }
         
        [HttpGet("info")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            throw new NotImplementedException();
        }
    }
}