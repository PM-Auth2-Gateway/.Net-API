using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Services.Abstract
{
    public interface IExchangeCodeForTokenService
    {
        public Task<string> ExchangeCodeForTokens(
            string tokenUri,
            IDictionary<string, string> queryParams,
            string socialName);
    }
}
