using System;
using System.Collections.Concurrent;
using System.Linq;

namespace PMAuth.Services.AuthAdmin
{
    public class RefreshTokenService
    {
        private readonly ConcurrentDictionary<string, RefreshTokenModel> refreshTokenDictionary = new ConcurrentDictionary<string, RefreshTokenModel>();

        public string GetRefreshToken(string login)
        {
            var removes= refreshTokenDictionary.Where(r => r.Value.TimeOut());
            removes.Select(r => refreshTokenDictionary.TryRemove(r.Key, out _));
            if (refreshTokenDictionary.TryGetValue(login, out RefreshTokenModel token))
            {
                return token.RefreshToken;
            }

            return null;
        }

        internal void DeleteRefreshToken(string username)
        {
            refreshTokenDictionary.TryRemove(username,out _);
        }

        internal void SaveRefreshToken(string username, string newRefreshToken)
        {
            if(!refreshTokenDictionary.TryAdd(username,new RefreshTokenModel(newRefreshToken,DateTime.UtcNow)))
            {
                refreshTokenDictionary[username] = new RefreshTokenModel(newRefreshToken, DateTime.UtcNow);
            }
        }
    } 
}
