using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace PMAuth.Services.AuthAdmin.Interface
{
    public interface IAuthServise
    {
        public ClaimsIdentity GetIdentity(string name);
        public string GenerateRefreshToken();

        public JwtSecurityToken GenerateToken(IEnumerable<Claim> claims);

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        public string CheckId(string adminName);

        public string CheckId(string adminName, int appId);

        public string CheckId(string adminName, int appId, int socialId);

    }
}
