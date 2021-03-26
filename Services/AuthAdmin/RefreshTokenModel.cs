using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Services.AuthAdmin
{
    public class RefreshTokenModel
    {
        public RefreshTokenModel(string refreshToken, DateTime createTime)
        {
            RefreshToken = refreshToken;
            CreateTime = createTime;
        }

        public string RefreshToken { get; set; }
        

        public DateTime ExpiresTime
        {
            get
            {
                return CreateTime.AddDays(60);
            }
        }
        public DateTime CreateTime { get; set; }

        public bool TimeOut()
        {
            if (ExpiresTime.CompareTo(DateTime.UtcNow) < 0)
                return true;
            return false;
        }
    }
}
