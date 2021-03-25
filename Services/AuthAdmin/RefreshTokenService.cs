using System.Collections.Concurrent;

namespace PMAuth.Services.AuthAdmin
{
    public class RefreshTokenService
    {
        private readonly ConcurrentDictionary<string, string> refreshTokenDictionary = new ConcurrentDictionary<string, string>();

        public string GetRefreshToken(string login)
        {
            if (refreshTokenDictionary.TryGetValue(login, out string token))
            {
                return token;
            }

            return null;
        }

        internal void DeleteRefreshToken(string username, string refreshToken)
        {
            refreshTokenDictionary.TryRemove(username,out _);
        }

        internal void SaveRefreshToken(string username, string newRefreshToken)
        {
            if(!refreshTokenDictionary.TryAdd(username, newRefreshToken))
            {
                refreshTokenDictionary[username] = newRefreshToken;
            }
        }
    } 
}
