using System;
using System.Collections.Concurrent;
using System.Linq;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;

namespace PMAuth.Services.AuthAdmin
{
    public class RefreshTokenService
    {
        private readonly BackOfficeContext _backOfficeContext;

        public RefreshTokenService(BackOfficeContext backOfficeContext)
        {
            _backOfficeContext = backOfficeContext;
        }

        internal bool CheckRefreshToken(string token)
        {
            return _backOfficeContext.RefreshTokens.FirstOrDefault(r=>r.RefreshTokenValue == token)!=null;
        }

        internal void DeleteRefreshToken(string token)
        {
            if (!CheckRefreshToken(token))
            {
                return;
            }

            var refresh = _backOfficeContext.RefreshTokens.FirstOrDefault(r => r.RefreshTokenValue == token);
            _backOfficeContext.RefreshTokens.Remove(refresh);
            _backOfficeContext.SaveChanges();
        }

        internal void SaveRefreshToken(string newRefreshToken)
        {
            _backOfficeContext.RefreshTokens.Add(new RefreshToken()
            {
                RefreshTokenValue = newRefreshToken,
                ExpiresTime = DateTime.UtcNow.AddDays(AuthOptions.LIFETIMEREFRESH)
            });

            _backOfficeContext.SaveChanges();
        }
    } 
}
