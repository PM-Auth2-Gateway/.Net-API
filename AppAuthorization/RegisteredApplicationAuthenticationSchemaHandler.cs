using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMAuth.AuthDbContext;
using PMAuth.Exceptions.Models;

#pragma warning disable 1591

namespace PMAuth.AppAuthorization
{
    public class RegisteredApplicationAuthenticationSchemaHandler  : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BackOfficeContext _context;

        public RegisteredApplicationAuthenticationSchemaHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            BackOfficeContext context) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("App_id") == false)
            {
                return await HandleAuthenticationFail("Missing App_id Header");
            }

            string appIdRaw = Request.Headers["App_id"];
            if (int.TryParse(appIdRaw, out int appId) == false)
            {
                return await HandleAuthenticationFail("Invalid App_id Header");
            }

            bool isRegistered = _context.Apps.Any(a => a.Id == appId);
            if (isRegistered == false)
            {
                return await HandleAuthenticationFail("Unregistered App_id");
            }
            
            Claim[] claims = {
                new Claim(ClaimTypes.Name, appIdRaw),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    
            return AuthenticateResult.Success(ticket);
        }

        private async Task<AuthenticateResult> HandleAuthenticationFail(string errorMessage)
        {
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            
            string result = JsonConvert.SerializeObject(ErrorModel.UnauthorizedAccessModel(errorMessage));
            Context.Response.ContentType = "application/json";
            await Context.Response.WriteAsync(result);
            
            return AuthenticateResult.Fail(errorMessage);
        }
    }
}