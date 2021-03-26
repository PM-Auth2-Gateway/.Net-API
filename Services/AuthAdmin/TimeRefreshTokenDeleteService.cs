using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMAuth.AuthDbContext;

namespace PMAuth.Services.AuthAdmin
{
    public class TimeRefreshTokenDeleteService:IHostedService,IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private int _period = 1;
        private Timer _timer;

        public TimeRefreshTokenDeleteService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken) 
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(_period));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var backOfficeContext = _scopeFactory.CreateScope().ServiceProvider.GetService<BackOfficeContext>();
            backOfficeContext.RefreshTokens.Where(r => (r.ExpiresTime.CompareTo(DateTime.UtcNow) < 0)).ToArray()
                .Select(r => backOfficeContext.RefreshTokens.Remove(r)).ToArray();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
