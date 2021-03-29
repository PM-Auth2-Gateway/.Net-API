using System;

namespace PMAuth.AuthDbContext.Entities
{
    /// <summary>
    /// Entity that handle refresh token and his expires time.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Refresh Token Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Handle Refresh Token Value
        /// </summary>
        public string RefreshTokenValue { get; set; }

        /// <summary>
        /// User that recieve a token
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Expires time of refresh token.
        /// </summary>
        public DateTime ExpiresTime { get; set; }

    }
}
