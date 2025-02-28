﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using PMAuth.AuthDbContext;
using PMAuth.Services.AuthAdmin.Interface;

namespace PMAuth.Services.AuthAdmin
{
    public class AuthService:IAuthServise
    {
        private readonly BackOfficeContext _backOfficeContext;

        public AuthService(BackOfficeContext backOfficeContext)
        {
            this._backOfficeContext = backOfficeContext;
        }
        public ClaimsIdentity GetIdentity(string name)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
           return new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                claims: claims, //the user's claims, for example new Claim[] { new Claim(ClaimTypes.Name, "The username"), //... 
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(AuthOptions.LIFETIME),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string CheckId(string adminName)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == adminName)?.Id;
            return adminId == null ? "This admin is not existed" : null;
        }
        public string CheckId(string adminName, int appId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == adminName)?.Id;
            if(adminId == null)
                return "This admin is not existed";
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            return resApp == null ? "This application is not existed" : null;

        }

        public string CheckId(string adminName, int appId, int socialId)
        {
            var adminId = _backOfficeContext.Admins.FirstOrDefault(a => a.Name == adminName)?.Id;
            if (adminId == null)
                return "This admin is not existed";
            var resApp = _backOfficeContext.Apps.FirstOrDefault(a => (a.Id == appId) && (a.AdminId == adminId));
            if (resApp == null)
                return "This application is not existed";
            var resSocial = _backOfficeContext.Socials.FirstOrDefault(a => a.Id == socialId);
            if (resSocial == null)
                return "This social is not existed";
            var set = _backOfficeContext.Settings.FirstOrDefault(s =>(s.AppId == appId) && (s.SocialId == socialId));
            return set == null ? "This setting is not existed" : null;
        }
    }
}
