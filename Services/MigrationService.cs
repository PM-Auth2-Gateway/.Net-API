using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMAuth.AuthDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMAuth.Services
{
    /// <summary>
    /// Migration service.
    /// </summary>
    public class MigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// MigrationService constructor.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider</param>
        public MigrationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Start migration service Async
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await using var productManagerContext = scope.ServiceProvider.GetRequiredService<BackOfficeContext>();
            await productManagerContext.Database.MigrateAsync(cancellationToken);
        }

        /// <summary>
        /// Stop migration service Async
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
