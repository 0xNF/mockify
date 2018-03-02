using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mockify.Services {

    public class RateLimitResetService : HostedService {

        private readonly IServiceScopeFactory scopeFactory;
        private static TimeSpan ts = TimeSpan.FromMinutes(1);
        private ILogger<RateLimitResetService> _logger;

        public RateLimitResetService(
            IServiceScopeFactory scopeFactory,
            ILogger<RateLimitResetService> logger) {
            this.scopeFactory = scopeFactory;
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {

            while (!cancellationToken.IsCancellationRequested) {
                try {
                    using (IServiceScope scope = scopeFactory.CreateScope()) {
                        MockifyDbContext mc = scope.ServiceProvider.GetRequiredService<MockifyDbContext>();
                        IQueryable<RateLimits> ExpiredLimits = mc.RateLimits.Where(x => (x.WindowStartTime + x.RateWindow) <= DateTime.UtcNow);
                        if(ExpiredLimits.Any()) {
                            /* All Expired Rate Limits */
                            foreach (RateLimits rl in ExpiredLimits) {
                                rl.CurrentCalls = 0;
                            }
                            await mc.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e) {
                    _logger.LogError("Failed to clear rate limits", e);
                }
                await Task.Delay(ts, cancellationToken);
            }
        }
    }
}
