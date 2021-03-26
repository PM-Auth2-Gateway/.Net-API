using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PMAuth.Services.AuthAdmin
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        const string KEY = "mysupersecret_secretkey!1232151525255_PM_Academy";
        public const int LIFETIME = 30; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
