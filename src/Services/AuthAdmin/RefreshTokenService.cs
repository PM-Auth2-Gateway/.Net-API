using System;
using System.Collections.Concurrent;
using System.Linq;
using PMAuth.AuthDbContext;
using PMAuth.AuthDbContext.Entities;
using PMAuth.Services.AuthAdmin.Interface;

namespace PMAuth.Services.AuthAdmin
{
    public class RefreshTokenService:IRefreshTokenService
    {
        private readonly BackOfficeContext _backOfficeContext;

        public RefreshTokenService(BackOfficeContext backOfficeContext)
        {
            _backOfficeContext = backOfficeContext;
        }

        public bool CheckRefreshToken(string userId,string token)
        {
            return _backOfficeContext.RefreshTokens.FirstOrDefault(r=>(r.RefreshTokenValue == token)&&(r.UserId == userId))!=null;
        }

        public void DeleteRefreshToken(string userId,string token)
        {
            if (!CheckRefreshToken(userId,token))
            {
                return;
            }

            var refresh = _backOfficeContext.RefreshTokens.FirstOrDefault(r => (r.RefreshTokenValue == token) && (r.UserId == userId));
            _backOfficeContext.RefreshTokens.Remove(refresh);
            _backOfficeContext.SaveChanges();
        }

        public void SaveRefreshToken(string userId,string newRefreshToken)
        {
            _backOfficeContext.RefreshTokens.Add(new RefreshToken()
            {
                UserId = userId,
                RefreshTokenValue = newRefreshToken,
                ExpiresTime = DateTime.UtcNow.AddDays(AuthOptions.LIFETIMEREFRESH)
            });

            _backOfficeContext.SaveChanges();
        }
    } 
}
