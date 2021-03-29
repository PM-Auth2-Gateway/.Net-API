namespace PMAuth.Services.AuthAdmin.Interface
{
    public interface IRefreshTokenService
    {
        public bool CheckRefreshToken(string userId, string token);
        public void DeleteRefreshToken(string userId, string token);
        public void SaveRefreshToken(string userId, string newRefreshToken);

    }
}
